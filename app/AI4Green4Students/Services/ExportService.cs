using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Utilities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AI4Green4Students.Services;

public class ExportService
{
  private readonly SectionService _sections;
  private readonly FieldService _fields;
  private readonly FieldResponseService _fieldResponses;
  private readonly AzureStorageService _azureStorageService;
  private static readonly List<string> _filteredFields = [InputTypes.Content, InputTypes.Header];

  public ExportService(SectionService sections, FieldService fields, FieldResponseService fieldResponses, AzureStorageService azureStorageService)
  {
    _sections = sections;
    _fields = fields;
    _fieldResponses = fieldResponses;
    _azureStorageService = azureStorageService;
  }

  /// <summary>
  /// Generates an export stream for a given project and section type.
  /// </summary>
  /// <param name="id">Record id.</param>
  /// <param name="projectId">Project id.</param>
  /// <param name="title">Document title. Defaults to "Title".</param>
  /// <param name="author">Document author. Defaults to "Author".</param>
  /// <returns>Document stream.</returns>
  public async Task<Stream> GeExportStream<T>(int id, int projectId, string title = "Title", string author = "Author") where T : BaseSectionTypeData
  {
    var exportModel = await GetExportModel<T>(id, projectId);

    var memoryStream = new MemoryStream();
    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
    {
      var mainPart = wordDocument.AddMainDocumentPart();
      mainPart.Document = new Document();
      var body = new Body();

      AppendTitleToBody(body, title, author);

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
  /// Get export model.
  /// </summary>
  /// <param name="id">Entity id to get export model for. E.g. Plan id</param>
  /// <param name="projectId">Project id. Ensures only fields and sections associated with the project are returned </param>
  /// <returns>Model for exporting</returns>
  private async Task<List<SectionExportModel>> GetExportModel<T>(int id, int projectId) where T : BaseSectionTypeData
  {
    var sectionType = typeof(T).Name switch
    {
      nameof(LiteratureReview) => SectionTypes.LiteratureReview,
      nameof(Plan) => SectionTypes.Plan,
      nameof(Note) => SectionTypes.Note,
      nameof(Report) => SectionTypes.Report,
      _ => throw new InvalidOperationException("Unsupported section type")
    };

    var fieldsResponses = await _fieldResponses.ListBySectionType<T>(id);
    var sections = await _sections.ListBySectionTypeName(sectionType, projectId);
    var fields = await _fields.ListBySectionType(sectionType, projectId);

    return sections.Select(x => new SectionExportModel
    {
      Id = x.Id,
      Name = x.Name,
      Fields = fields
        .Where(f => f.Section.Id == x.Id && !_filteredFields.Contains(f.InputType.Name))
        .Select(f => new ExportFieldModel
        {
          Id = f.Id,
          Name = f.Name,
          Type = f.InputType.Name,
          SelectFieldOptions = f.SelectFieldOptions.Count >= 1
            ? f.SelectFieldOptions
              .Select(option => new SelectFieldOptionModel(option))
              .ToList()
            : null,
          Response = fieldsResponses
            .Where(y => y.Field.Id == f.Id)
            .Select(y => y.FieldResponseValues.MaxBy(z => z.ResponseDate)?.Value)
            .SingleOrDefault() ?? f.DefaultResponse
        }).ToList()
    }).ToList();
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

    var reportTitle =
      CreateFormattedRun(title, ExportDefinitions.PrimaryHeadingFontSize, ExportDefinitions.FontFace, true);
    var titleParagraph = CreateFormattedParagraph(reportTitle);
    titleParagraph.ParagraphProperties =
      new ParagraphProperties(new Justification { Val = JustificationValues.Center });
    body.Append(titleParagraph);

    if (author is not null)
    {
      var authorName = CreateFormattedRun(author, ExportDefinitions.SecondaryHeadingFontSize,
        ExportDefinitions.FontFace, false);
      var authorParagraph = CreateFormattedParagraph(authorName);
      authorParagraph.ParagraphProperties =
        new ParagraphProperties(new Justification { Val = JustificationValues.Center });
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
      case InputTypes.FormattedTextInput:
        // Append the formatted HTML text to the Word document body
        AppendFormattedTextToBody(body, field.Response);
        break;
      case InputTypes.Description:
      case InputTypes.Text:
        // Field name
        body.Append(CreateFormattedParagraph(
          CreateFormattedRun(field.Name, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));

        // Field response
        body.Append(CreateFormattedParagraph(
          CreateFormattedRun(field.Response, ExportDefinitions.FieldResponseFontSize, ExportDefinitions.FontFace,
            false)));

        // Space after
        body.Append(new Break());
        break;

      case InputTypes.ImageFile:
        var images = SerializerHelper.DeserializeOrDefault<List<FileInputTypeModel>>(field.Response);
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
        var sortableListItems =
          SerializerHelper.DeserializeOrDefault<List<SortableListItemInputTypeModel>>(field.Response);
        if (sortableListItems is not null)
        {
          // Field name
          body.Append(CreateFormattedParagraph(
            CreateFormattedRun(field.Name, ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace, true)));

          // List items
          foreach (var item in sortableListItems)
          {
            body.Append(CreateFormattedParagraph(
              CreateFormattedRun($"{item.Order}. {item.Content}", ExportDefinitions.FieldResponseFontSize,
                ExportDefinitions.FontFace, false)));
          }

          // Space after list
          body.Append(new Break());
        }

        break;

      case InputTypes.MultiYieldTable:
        var multiYieldTables =
          SerializerHelper.DeserializeOrDefault<List<MultiYieldTableInputTypeModel>>(field.Response);
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
              AddCellToRow(row,
                data.ExpectedMass is not null ? $"{data.ExpectedMass.Value} {data.ExpectedMass.Unit}" : string.Empty);
              AddCellToRow(row,
                data.ActualMass is not null ? $"{data.ActualMass.Value} {data.ActualMass.Unit}" : string.Empty);
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
        var multiGreenMetricsTables =
          SerializerHelper.DeserializeOrDefault<List<MultiGreenMetricsInputTypeModel>>(field.Response);
        if (multiGreenMetricsTables is null) return;
        foreach (var greenMetricsTable in multiGreenMetricsTables)
        {
          var sourceName = $"Source: {greenMetricsTable.Source.Name}";
          var sourceType = $"Source type: {greenMetricsTable.Source.Type}";
          var caption = $"{sourceName}, {sourceType}";

          // Title
          body.Append(CreateFormattedParagraph(
            CreateFormattedRun("Green Metrics Calculation", ExportDefinitions.FieldNameFontSize,
              ExportDefinitions.FontFace, true)));
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
        var multiReactionScheme =
          SerializerHelper.DeserializeOrDefault<List<MultiReactionSchemeInputTypeModel>>(field.Response);

        if (multiReactionScheme is not null)
          foreach (var reactionScheme in multiReactionScheme)
          {
            var image = reactionScheme.Data.ReactionSketch.ReactionImage;

            // caption with source name and type
            var sourceName = $"Source: {reactionScheme.Source.Name}";
            var sourceType = $"Source type: {reactionScheme.Source.Type}";

            // Image
            if (image is not null)
            {
              body.Append(CreateFormattedParagraph(
                CreateFormattedRun("Reaction Scheme", ExportDefinitions.FieldNameFontSize, ExportDefinitions.FontFace,
                  true)));

              body.Append(new Paragraph(
                new Run(await ProcessImage(image.Location, GetImagePartType(image.Name), mainPart))));

              var imgCaption = $"{sourceName}, {sourceType}";
              body.Append(CreateFormattedParagraph(
                CreateFormattedRun(imgCaption, ExportDefinitions.CaptionFontSize, ExportDefinitions.FontFace, false)));

              body.Append(new Break());
            }
          }
        break;

      // TODO: Add more field types if needed.
      default: return;
    }

    body.Append(new Break());
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
  /// Return a paragraph with a page break.
  /// </summary>
  /// <returns>Page break paragraph.</returns>
  private static Paragraph CreatePageBreak()
  {
    var breakParagraph = new Paragraph(new Run(new Break { Type = BreakValues.Page }));
    return breakParagraph;
  }

  /// <summary>
  /// Appends formatted HTML text to the Word document body.
  /// </summary>
  /// <param name="body">The body of the Word document where the formatted text will be appended.</param>
  /// <param name="htmlText">The HTML formatted text to be converted and appended.</param>
  private void AppendFormattedTextToBody(Body body, string htmlText)
  {
    // Replace &nbsp; with a regular space
    htmlText = htmlText.Replace("&nbsp;", " ");

    var doc = new HtmlDocument();
    doc.LoadHtml(SerializerHelper.DeserializeOrDefault<string>(htmlText));

    foreach (var node in doc.DocumentNode.ChildNodes)
    {
      ProcessHtmlNode(node, body);
    }
  }

  /// <summary>
  /// Processes an HTML node and appends corresponding OpenXml elements to the parent element.
  /// Supports various HTML tags such as paragraphs, bold, italic, underline, strikethrough,
  /// subscript, superscript, and text nodes, mapping them to OpenXml equivalents.
  /// </summary>
  /// <param name="node">The HTML node to process.</param>
  /// <param name="parentElement">The parent OpenXml element to which the processed content will be appended.</param>
  /// <param name="parentRun">Optional. The parent run element for inline styling, defaulting to a new Run if not provided.</param>
  private static void ProcessHtmlNode(HtmlNode node, OpenXmlElement parentElement, Run? parentRun = null)
  {
    var run = parentRun ?? new Run();

    switch (node.Name)
    {
      case "p":
        var paragraph = new Paragraph();
        parentElement.Append(paragraph);
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, paragraph);
        break;

      case "b":
      case "strong":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.Bold = new Bold();
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "em":
      case "i":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.Italic = new Italic();
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "u":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.Underline = new Underline() { Val = UnderlineValues.Single };
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "s":
      case "strike":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.Strike = new Strike();
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "sub":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.VerticalTextAlignment = new VerticalTextAlignment { Val = VerticalPositionValues.Subscript };
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "sup":
        if (run.RunProperties is null) run.RunProperties = new RunProperties();
        run.RunProperties.VerticalTextAlignment = new VerticalTextAlignment
          { Val = VerticalPositionValues.Superscript };
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;

      case "#text":
        var text = new Text(node.InnerText);
        if (node.InnerText.Contains(' ')) text.Space = SpaceProcessingModeValues.Preserve;
        run.Append(text);
        parentElement.Append(run);
        break;

      default:
        foreach (var childNode in node.ChildNodes) ProcessHtmlNode(childNode, parentElement, run);
        break;
    }
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
    var imageData = await _azureStorageService.Get(imageLocation);

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
      new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
        new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = cx, Cy = cy },
        new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent
          { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
        new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties { Id = (UInt32Value)1U, Name = "Picture 1" },
        new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
          new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks { NoChangeAspect = true }),
        new DocumentFormat.OpenXml.Drawing.Graphic(
          new DocumentFormat.OpenXml.Drawing.GraphicData(
            new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
              new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties
                  { Id = (UInt32Value)0U, Name = "New Bitmap Image.png" },
                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
              ),
              new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                new DocumentFormat.OpenXml.Drawing.Blip(
                  new DocumentFormat.OpenXml.Drawing.BlipExtensionList(
                    new DocumentFormat.OpenXml.Drawing.BlipExtension { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }
                  )
                )
                {
                  Embed = relationshipId,
                  CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                },
                new DocumentFormat.OpenXml.Drawing.Stretch(new DocumentFormat.OpenXml.Drawing.FillRectangle())
              ),
              new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                new DocumentFormat.OpenXml.Drawing.Transform2D(
                  new DocumentFormat.OpenXml.Drawing.Offset { X = 0L, Y = 0L },
                  new DocumentFormat.OpenXml.Drawing.Extents { Cx = cx, Cy = cy }),
                new DocumentFormat.OpenXml.Drawing.PresetGeometry(new DocumentFormat.OpenXml.Drawing.AdjustValueList())
                  { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle }
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
}
