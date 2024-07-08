using System.Globalization;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Report;
using AI4Green4Students.Models.Section;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace AI4Green4Students.Services;

public class ReportService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionFormService _sectionForm;
  private readonly AZExperimentStorageService _azStorage;

  public ReportService(ApplicationDbContext db, StageService stageService, SectionFormService sectionForm, AZExperimentStorageService azStorage)
  {
    _db = db;
    _stages = stageService;
    _sectionForm = sectionForm;
    _azStorage = azStorage;
  }

  /// <summary>
  /// Get a list of project reports for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get reports for.</param>
  /// <param name="userId">Id of the user to get reports for.</param>
  /// <returns>List of project report of the user.</returns>
  public async Task<List<ReportModel>> ListByUser(int projectId, string userId)
  {
    var reports = await ReportsQuery().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == projectId).ToListAsync();

    var list = new List<ReportModel>();

    foreach (var report in reports)
    {
      var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report);
      var model = new ReportModel(report)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list; 
  }

  /// <summary>
  /// Get student reports for a project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to check reports for.</param>
  /// <returns>List of project reports for given project group.</returns>
  public async Task<List<ReportModel>> ListByProjectGroup(int projectGroupId)
  {
    var pgStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Students)
      .Where(x => x.Id == projectGroupId)
      .SelectMany(x => x.Students)
      .ToListAsync();
    
    var reports = await ReportsQuery().AsNoTracking().Where(x => pgStudents.Contains(x.Owner)).ToListAsync();
    
    var list = new List<ReportModel>();

    foreach (var report in reports)
    {
      var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report);
      var model = new ReportModel(report)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }

  /// <summary>
  /// Get a report by its id.
  /// </summary>
  /// <param name="id">Id of the report</param>
  /// <returns>Report matching the id.</returns>
  public async Task<ReportModel> Get(int id)
  {
    var report = await ReportsQuery().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();
    
    var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report); 
    return new ReportModel(report)
    {
      Permissions = permissions
    };
  }

  /// <summary>
  /// Create a new report.
  /// Before creating a report, check if the user is a member of the project group.
  /// </summary>
  /// <param name="ownerId">Id of the user creating the report.</param>
  /// <param name="model">Report dto model. Currently only contains project group id.</param>
  /// <returns>Newly created report.</returns>
  public async Task<ReportModel> Create(string ownerId, CreateReportModel model)
  {
    var user = await _db.Users.FindAsync(ownerId)
               ?? throw new KeyNotFoundException();

    var projectGroup = await _db.ProjectGroups
                         .Where(x => x.Id == model.ProjectGroupId && x.Students.Any(y => y.Id == ownerId))
                         .Include(x=>x.Project)
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();
    
    var existing = await _db.Reports
      .Where(x => x.Owner.Id == ownerId && x.Project.Id == projectGroup.Project.Id)
      .FirstOrDefaultAsync();

    if (existing is not null) return await Get(existing.Id); // Only one report allowed to a user for a project.

    var draftStage = await _db.Stages.SingleAsync(x => x.DisplayName == ReportStages.Draft && x.Type.Value == StageTypes.Report);
    
    var entity = new Report { Title = model.Title, Owner = user, Project = projectGroup.Project, Stage = draftStage };
    await _db.Reports.AddAsync(entity);
    
    entity.FieldResponses = await _sectionForm.CreateFieldResponse<Report>(entity.Id, projectGroup.Project.Id, SectionTypes.Report, null);
    
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete report by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the report for.</param>
  /// <param name="id">The id of a report to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Reports
                   .SingleOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();

    _db.Reports.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given report.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="reportId">Id of the report to check the user against.</param>
  /// <returns>True if the user is the owner of the report, false otherwise.</returns>
  public async Task<bool> IsReportOwner(string userId, int reportId)
    => await _db.Reports
      .AsNoTracking()
      .AnyAsync(x => x.Id == reportId && x.Owner.Id == userId);

  public async Task<ReportModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _stages.AdvanceStage<Report>(id, StageTypes.Report, setStage);

    if (entity?.Stage is null) return null;

    var stagePermission = await _stages.GetStagePermissions(entity.Stage, StageTypes.Report);
    return new ReportModel(entity) { Permissions = stagePermission };
  }
  
  /// <summary>
  /// Get section summaries for a given report.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="reportId">Id of the report to be used when processing the summaries</param>
  /// <param name="sectionTypeId">Id of the section type.</param>
  /// <returns>Section summaries</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int reportId, int sectionTypeId)
  {
    var report = await Get(reportId);
    var fieldsResponses = await _sectionForm.ListBySectionType<Report>(reportId);
    return await _sectionForm.GetSummaryModel(sectionTypeId, fieldsResponses, report.Permissions, report.Stage);
  }
  
  /// <summary>
  /// Get a report section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="reportId">Id of the report to get the field responses for</param>
  /// <returns>Report section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int reportId, int sectionId)
  {
    var fieldsResponses = await _sectionForm.ListBySection<Report>(reportId, sectionId);
    return await _sectionForm.GetFormModel(sectionId, fieldsResponses);
  }
  
  /// <summary>
  /// Save report section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _sectionForm.GenerateFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _sectionForm.GenerateFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
    };
    
    var report = await Get(model.RecordId);
    var fieldResponses = await _sectionForm.ListBySection<Report>(submission.RecordId, submission.SectionId);

    var updatedValues= report.Stage == ReportStages.Draft
      ? _sectionForm.UpdateDraftFieldResponses(submission.FieldResponses, fieldResponses)
      : _sectionForm.UpdateAwaitingChangesFieldResponses(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Reports.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _sectionForm.CreateFieldResponse<Report>(report.Id, report.ProjectId, SectionTypes.Report, submission.NewFieldResponses);
    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm(model.RecordId, model.SectionId);
  }
  
  /// <summary>
  /// Generate a Word document for a given report.
  /// </summary>
  /// <param name="reportId">Report id.</param>
  /// <returns>Word document as a byte array.</returns>
  public async Task<Stream> GenerateExport(int reportId)
  {
    var report = await Get(reportId);
    var exportModel = await _sectionForm.GetExportModel<Report>(reportId, report.ProjectId, SectionTypes.Report);

    var memoryStream = new MemoryStream();
    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
    {
      var mainPart = wordDocument.AddMainDocumentPart();
      mainPart.Document = new Document();
      var body = new Body();
      
      AppendTitleToBody(body, report.Title, report.OwnerName); // Title and author
        
      foreach (var section in exportModel)
      {
        AppendSectionToBody(body, section.Name); // Section heading
        foreach (var field in section.Fields)  
          await AppendFieldToBody(mainPart, body, field); // Section fields and responses 
        body.Append(CreatePageBreak());
      }
      
      mainPart.Document.Append(body);
      mainPart.Document.Save();
    }
    memoryStream.Position = 0;
    return memoryStream;
  }
  
  /// <summary>
  /// Construct a query to fetch Report along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Report entities.</returns>
  private IQueryable<Report> ReportsQuery()
  {
    return _db.Reports
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage);
  }
  
  /// <summary>
  /// Create a run with the given text.
  /// </summary>
  /// <param name="text"> Text to be added to the run. </param>
  /// <param name="fontSize"> Font size. </param>
  /// <param name="fontFace"> Font face. </param>
  /// <param name="isBold">Bold text or not.</param>
  /// <returns>Run with the given text.</returns>
  private static Run CreateFormattedRun(string text, string fontSize, string fontFace, bool isBold)
  {
    var run = new Run(new Text(text))
    {
      RunProperties = new RunProperties
      {
        FontSize = new FontSize { Val = fontSize },
        RunFonts = new RunFonts { Ascii = fontFace }
      }
    };
    if (isBold)
    {
      run.RunProperties.Bold = new Bold();
    }
    return run;
  }

  /// <summary>
  /// Create a paragraph with the given run.
  /// </summary>
  /// <param name="run">Run to be added to the paragraph.</param>
  /// <returns>Paragraph for the given run.</returns>
  private static Paragraph CreateFormattedParagraph(Run run)
  {
    var paragraph = new Paragraph();
    paragraph.Append(run);
    return paragraph;
  }

  /// <summary>
  /// Append a title.
  /// </summary>
  /// <param name="body">Document body.</param>
  /// <param name="title">Report title.</param>
  /// <param name="author">Report author.</param>
  private static void AppendTitleToBody(Body body, string title, string? author = null)
  {
    for (var i = 0; i < 10; i++) body.Append(new Paragraph()); // Spaces before title
    
    var reportTitle = CreateFormattedRun(title, ExportDefinitions.PrimaryHeadingFontSize, ExportDefinitions.FontFace, true);
    var titleParagraph = CreateFormattedParagraph(reportTitle);
    titleParagraph.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
    body.Append(titleParagraph);

    if (author is not null)
    {
      var authorName = CreateFormattedRun(author, ExportDefinitions.SecondaryHeadingFontSize, ExportDefinitions.FontFace, false);
      var authorParagraph = CreateFormattedParagraph(authorName);
      authorParagraph.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
      body.Append(authorParagraph);
    }

    body.Append(CreatePageBreak());
  }
  
  /// <summary>
  /// Append a section heading to the body of the document, including a page break before each section.
  /// </summary>
  /// <param name="body">Document body.</param>
  /// <param name="sectionName">Section title to be added.</param>
  private static void AppendSectionToBody(Body body, string sectionName)
  {
    var sectionParagraph = CreateFormattedRun(sectionName, ExportDefinitions.SectionHeadingFontSize, ExportDefinitions.FontFace, true);
    body.Append(CreateFormattedParagraph(sectionParagraph));

    // Add space after section heading
    var spaceAfterHeading = new Paragraph();
    body.Append(spaceAfterHeading);
  }
  
  /// <summary>
  /// Append a field to the body of the document.
  /// </summary>
  /// <param name="mainPart">Main document part.</param>
  /// <param name="body">Document body.</param>
  /// <param name="field">Field to be added to the body.</param>
  private async Task AppendFieldToBody(MainDocumentPart mainPart, Body body, ExportFieldModel field)
  {
    switch (field.Type)
    {
      case InputTypes.Description:
      case InputTypes.Text:
        // Field name
        body.Append(CreateFormattedParagraph(
          CreateFormattedRun(field.Name, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));
        
        // Field response
        body.Append(CreateFormattedParagraph(
          CreateFormattedRun(field.Response, ExportDefinitions.FieldResponseFontSize, ExportDefinitions.FontFace, false)));
        
        // Space after
        body.Append(new Break());
        break;
      
      case InputTypes.ImageFile:
        var images = _sectionForm.DeserialiseSafely<List<FileInputTypeModel>>(field.Response);
        if (images is not null && images.Count > 0)
        {
          foreach (var image in images)
          {
            // Image
            body.Append(new Paragraph(
              new Run(await ProcessImage(image.Location, GetImagePartType(image.Name), mainPart))));
            
            // Image caption
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun(image.Name, ExportDefinitions.CaptionFontSize, ExportDefinitions.FontFace, false)));

            // Space after image
            body.Append(new Break());
          }
        }
        break;
      
      case InputTypes.SortableList:
        var sortableListItems = _sectionForm.DeserialiseSafely<List<SortableListItemInputTypeModel>>(field.Response);
        if (sortableListItems is not null)
        {
          // Field name
          body.Append(CreateFormattedParagraph(
            CreateFormattedRun(field.Name, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));
          
          // List items
          foreach (var item in sortableListItems)
          {
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun($"{item.Order}. {item.Content}", ExportDefinitions.FieldResponseFontSize, ExportDefinitions.FontFace, false)));
          }
          
          // Space after list
          body.Append(new Break());
        }
        break;

      case InputTypes.MultiYieldTable:
        var multiYieldTables = _sectionForm.DeserialiseSafely<List<MultiYieldTableInputTypeModel>>(field.Response);
        var yieldTableColumnHeaders = YieldTable.ColumnHeaders;

        if (multiYieldTables is not null)
          foreach (var yieldTable in multiYieldTables)
          {
            var table = CreateTable();
            var headerRow = new TableRow();
            AddCellToRow(headerRow, "No.");
            foreach (var header in yieldTableColumnHeaders) AddCellToRow(headerRow, header, true);
            table.Append(headerRow);
  
            foreach (var data in yieldTable.Data)
            {
              var row = new TableRow();
              AddCellToRow(row, data.SerialNumber.ToString());
              AddCellToRow(row, data.Product);
              AddCellToRow(row, data.ExpectedMass is not null ? $"{data.ExpectedMass.Value} {data.ExpectedMass.Unit}" : string.Empty);
              AddCellToRow(row, data.ActualMass is not null ? $"{data.ActualMass.Value} {data.ActualMass.Unit}" : string.Empty);
              AddCellToRow(row, data.Moles.ToString());
              AddCellToRow(row, data.Yield.ToString());
              table.Append(row);
            }
  
            // Field name
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun(field.Name, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));
  
            // Table
            body.Append(table);
  
            // Table caption with source name and type
            var sourceName = $"Source: {yieldTable.Source.Name}";
            var sourceType = $"Source type: {yieldTable.Source.Type}";
            var tableCaption = $"{sourceName}, {sourceType}";
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun(tableCaption, ExportDefinitions.CaptionFontSize, ExportDefinitions.FontFace, false)));
  
            // Add space after table
            body.Append(new Break());
          }
        break;
      case InputTypes.MultiGreenMetricsTable:
        var multiGreenMetricsTables = _sectionForm.DeserialiseSafely<List<MultiGreenMetricsInputTypeModel>>(field.Response);
        if (multiGreenMetricsTables is null) return;
        foreach (var greenMetricsTable in multiGreenMetricsTables)
        {
          var sourceName = $"Source: {greenMetricsTable.Source.Name}";
          var sourceType = $"Source type: {greenMetricsTable.Source.Type}";
          var caption = $"{sourceName}, {sourceType}";
          
          // Title
          body.Append(CreateFormattedParagraph(
            CreateFormattedRun("Green Metrics Calculation", ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));
          body.Append(CreateFormattedParagraph(
            CreateFormattedRun(caption, ExportDefinitions.CaptionFontSize, ExportDefinitions.FontFace, false)));
          body.Append(new Break());
          
          // Waste Intensity
          var wiData = greenMetricsTable.Data.WasteIntensityCalculation;
          AppendMetricTableToBody(body, WasteIntensity.Title, WasteIntensity.ColumnHeaders, new[]
          {
            wiData?.Waste.ToString(),
            wiData?.Output.ToString(),
            wiData?.WasteIntensity.ToString()
          });
          
          // E-factor
          var efData = greenMetricsTable.Data.EfactorCalculation;
          AppendMetricTableToBody(body, Efactor.Title, Efactor.ColumnHeaders, new[]
          {
            efData?.WasteMass.ToString(),
            efData?.ProductMass.ToString(),
            efData?.Efactor.ToString()
          });
          
          // Reaction Mass Efficiency
          var rmeData = greenMetricsTable.Data.RmeCalculation;
          AppendMetricTableToBody(body, Rme.Title, Rme.ColumnHeaders, new[]
          {
            rmeData?.ProductMass.ToString(),
            rmeData?.ReactantMass.ToString(),
            rmeData?.Rme.ToString()
          });
          
          // Process Mass Intensity
          var pmiData = greenMetricsTable.Data.PmiCalculation;
          AppendMetricTableToBody(body, Pmi.Title, Pmi.ColumnHeaders, new[]
          {
            pmiData?.TotalMassInProcess.ToString(),
            pmiData?.ProductMass.ToString(),
            pmiData?.Pmi.ToString()
          });
        }
        break;
      
      case InputTypes.MultiReactionScheme:
        var multiReactionScheme = _sectionForm.DeserialiseSafely<List<MultiReactionSchemeInputTypeModel>>(field.Response);
        var rSchemeTableColumnHeaders = ReactionTable.ColumnHeaders;

        if (multiReactionScheme is not null)
          foreach (var reactionScheme in multiReactionScheme)
          {
            var table = CreateTable();
            var headerRow = new TableRow();
            foreach (var header in rSchemeTableColumnHeaders) AddCellToRow(headerRow, header, true);
            table.Append(headerRow);
  
            foreach (var data in reactionScheme.Data.ReactionTable)
            {
              var row = new TableRow();
              AddCellToRow(row, data.SubstanceType);
              AddCellToRow(row, data.SubstancesUsed);
              AddCellToRow(row, data.Limiting is true ? "Yes" : "No");
              AddCellToRow(row, data.Mass is not null ? $"{data.Mass.Value} {data.Mass.Unit}" : string.Empty);
              AddCellToRow(row, data.Gls);
              AddCellToRow(row, data.MolWeight?.ToString());
              AddCellToRow(row, data.Amount?.ToString());
              AddCellToRow(row, data.Density?.ToString());
              AddCellToRow(row, data.HazardsInput);
              table.Append(row);
            }
  
            // Field name
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun(ReactionTable.Title, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));
  
            // Table
            body.Append(table);
  
            // Table caption with source name and type
            var sourceName = $"Source: {reactionScheme.Source.Name}";
            var sourceType = $"Source type: {reactionScheme.Source.Type}";
            var tableCaption = $"{sourceName}, {sourceType}";
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun(tableCaption, ExportDefinitions.CaptionFontSize, ExportDefinitions.FontFace, false)));
  
            // Add space after table
            body.Append(new Break());
          }
        break;
      
      // TODO: Add more field types if needed.
      default: return;
    }
    body.Append(new Break());
  }
  
  /// <summary>
  /// Return a paragraph with a page break.
  /// </summary>
  /// <returns>Page break paragraph.</returns>
  private static Paragraph CreatePageBreak()
  {
    var breakParagraph = new Paragraph(new Run(new Break { Type = BreakValues.Page }));
    return breakParagraph;
  }

  /// <summary>
  /// Process the image to be added to the document.
  /// </summary>
  /// <param name="imageLocation">Image location in the storage.</param>
  /// <param name="imagePartType">Image part type based on the extension.</param>
  /// <param name="mainPart">Main document part.</param>
  /// <returns>Returns the drawing element for the image.</returns>
  private async Task<Drawing> ProcessImage(string imageLocation, ImagePartType imagePartType, MainDocumentPart mainPart)
  {
    var imageData = await _azStorage.Get(imageLocation);

    var img = Image.Load<Rgba32>(imageData);
    var aspectRatio = (double)img.Width / img.Height;
    var targetHeightPx = (int)(ExportDefinitions.ImageWidthPixels / aspectRatio);
    var widthEmus = (long)(ExportDefinitions.ImageWidthPixels * 9525);
    var heightEmus = (long)(targetHeightPx * 9525);

    var imagePart = mainPart.AddImagePart(imagePartType);

    using (var stream = new MemoryStream(imageData)) imagePart.FeedData(stream);

    var relationshipId = mainPart.GetIdOfPart(imagePart);
    return CreateDrawingElement(relationshipId, widthEmus, heightEmus);
  }

  /// <summary>
  /// Set up the drawing element for the image.
  /// </summary>
  /// <param name="relationshipId">Image relationship id for linking the image to the document.</param>
  /// <param name="widthEmus">Width to set for the image.</param>
  /// <param name="heightEmus">Height to set for the image.</param>
  /// <returns>Drawing element for the image.</returns>
  private static Drawing CreateDrawingElement(string relationshipId, long widthEmus, long heightEmus)
  {
    var cx = widthEmus;
    var cy = heightEmus;

    return new Drawing(
      new DW.Inline(
        new DW.Extent { Cx = cx, Cy = cy },
        new DW.EffectExtent { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
        new DW.DocProperties { Id = (UInt32Value)1U, Name = "Picture 1" },
        new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
        new A.Graphic(
          new A.GraphicData(
            new PIC.Picture(
              new PIC.NonVisualPictureProperties(
                new PIC.NonVisualDrawingProperties { Id = (UInt32Value)0U, Name = "New Bitmap Image.png" },
                new PIC.NonVisualPictureDrawingProperties()
              ),
              new PIC.BlipFill(
                new A.Blip(
                  new A.BlipExtensionList(
                    new A.BlipExtension { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }
                  )
                )
                {
                  Embed = relationshipId,
                  CompressionState = A.BlipCompressionValues.Print
                },
                new A.Stretch(new A.FillRectangle())
              ),
              new PIC.ShapeProperties(
                new A.Transform2D(new A.Offset { X = 0L, Y = 0L }, new A.Extents { Cx = cx, Cy = cy }),
                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
              )
            )
          ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
        )
      )
      {
        DistanceFromTop = (UInt32Value)0U,
        DistanceFromBottom = (UInt32Value)0U,
        DistanceFromLeft = (UInt32Value)0U,
        DistanceFromRight = (UInt32Value)0U,
        EditId = "50D07946"
      });
  }

  /// <summary>
  /// Get the image part type based on image name provided.
  /// </summary>
  /// <param name="imageName">Name to extract the extension from.</param>
  /// <returns>Image part type based on the extension.</returns>
  private static ImagePartType GetImagePartType(string imageName)
  {
      var extension = Path.GetExtension(imageName);
      return extension.ToLower() switch
      {
          ".jpeg" or ".jpg" => ImagePartType.Jpeg,
          ".png" => ImagePartType.Png,
          ".gif" => ImagePartType.Gif,
          ".bmp" => ImagePartType.Bmp,
          ".tiff" => ImagePartType.Tiff,
          ".ico" => ImagePartType.Icon,
          _ => ImagePartType.Png
      };
  }

  /// <summary>
  /// Create a table with predefined properties.
  /// </summary>
  /// <returns>Table.</returns>
  private static Table CreateTable()
  {
    var table = new Table();

    // table properties
    var tblProperties = new TableProperties(
      new TableBorders(
        new TopBorder { Val = BorderValues.Single, Size = 6 },
        new BottomBorder { Val = BorderValues.Single, Size = 6 },
        new LeftBorder { Val = BorderValues.Single, Size = 6 },
        new RightBorder { Val = BorderValues.Single, Size = 6 },
        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
        new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
      )
    );
    table.AppendChild(tblProperties);
    return table;
  }

  /// <summary>
  /// Add a cell to a row with the given text.
  /// </summary>
  /// <param name="row">Table row to add the cell to.</param>
  /// <param name="text">Text to be added to the cell.</param>
  /// <param name="isBold">Bold text or not.</param>
  private static void AddCellToRow(TableRow row, string? text, bool isBold = false)
  {
    var cell = new TableCell(CreateFormattedParagraph(
      CreateFormattedRun(text ?? string.Empty, ExportDefinitions.FieldResponseFontSize, ExportDefinitions.FontFace,
        isBold)));
    row.Append(cell);
  }
  
  /// <summary>
  /// Append a metric table to the body of the document.
  /// </summary>
  /// <param name="body">Document body. </param>
  /// <param name="title">Metric table title.</param>
  /// <param name="columnHeaders">Table column headers.</param>
  /// <param name="dataValues">Table data. </param>
  /// <remarks>Ensure that the dataValues array is in the same order as the columnHeaders array.</remarks>
  private static void AppendMetricTableToBody(Body body, string title, string[] columnHeaders, string?[] dataValues)
  {
    var table = CreateTable();
    var headerRow = new TableRow();
    foreach (var header in columnHeaders) AddCellToRow(headerRow, header, true);
    table.Append(headerRow);
    
    var dataRow = new TableRow();
    foreach (var value in dataValues) AddCellToRow(dataRow, value);
    table.Append(dataRow);
    
    body.Append(CreateFormattedParagraph(
      CreateFormattedRun(title, ExportDefinitions.FieldNameSecondaryFontSize, ExportDefinitions.FontFace, true)));
    body.Append(table);
    body.Append(new Break());
  }
}
