using AutoMapper;
using CitizenAppeals.Server.Model;
using CitizenAppeals.Shared.Dto;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitizenAppeals.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppealsController : ControllerBase
{
    private readonly CitizenAppealsContext _context;
    private readonly IMapper _mapper;

    public AppealsController(CitizenAppealsContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppeals()
    {
        try
        {
            var query = await _context.Appeals
                .Include(a => a.Citizen)
                .Include(a => a.Executors).ToListAsync();
            if (!User.IsInRole("Admin")) query = query.Where(a => a.ViolationType > 2).ToList();

            var appeals = query;
            var appealDtos = _mapper.Map<List<AppealDto>>(appeals);
            return Ok(appealDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{ex.Message}");
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCheckResult([FromBody] UpdateCheckResultDto dto)
    {
        try
        {
            var query = _context.Appeals.Where(a => dto.Ids!.Contains(a.Id));
            if (!User.IsInRole("Admin")) 
                query = query.Where(a => a.ViolationType > 2);
            var appeals = await query.ToListAsync();
            foreach (var appeal in appeals) 
                appeal.Result = dto.CheckResult;
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{ex.Message}");
        }
    }

    [HttpPost("report")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDto request)
    {
        try
        {
            var query = _context.Appeals
                .Where(a => a.AppealDate >= request.StartDate && a.AppealDate <= request.EndDate);

            if (!User.IsInRole("Admin")) query = query.Where(a => a.ViolationType > 2);

            var appeals = await query.ToListAsync();

            var reportData = appeals
                .GroupBy(a => a.ViolationType)
                .Select(g => new
                {
                    ViolationType = g.Key,
                    Detected = g.Count(a => a.Result == "Выявлено"),
                    NotDetected = g.Count(a => a.Result == "Не выявлено")
                })
                .OrderBy(x => x.ViolationType)
                .ToList();

            using var ms = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body;

                body.Append(new Paragraph(new Run(new Text(
                    $"Сформированный отчет по нарушениям с {request.StartDate:dd.MM.yyyy} по {request.EndDate:dd.MM.yyyy}"))));

                var table = new Table();
                var props = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) }
                    )
                );
                table.Append(props);

                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("")))),
                    new TableCell(new Paragraph(new Run(new Text("Выявлено")))),
                    new TableCell(new Paragraph(new Run(new Text("Не выявлено"))))
                );
                table.Append(headerRow);

                foreach (var data in reportData)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(
                            new Paragraph(new Run(new Text($"Характер выявленных нарушений {data.ViolationType}")))),
                        new TableCell(new Paragraph(new Run(new Text(data.Detected.ToString())))),
                        new TableCell(new Paragraph(new Run(new Text(data.NotDetected.ToString()))))
                    );
                    table.Append(row);
                }

                body.Append(table);
            }

            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "report.docx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{ex.Message}");
        }
    }
}