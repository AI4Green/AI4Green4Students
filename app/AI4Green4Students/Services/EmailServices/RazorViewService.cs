using AI4Green4Students.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AI4Green4Students.Services.EmailServices
{
  public class RazorViewService
  {
    private readonly ActionContext _actionContext;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IRazorViewEngine _razor;

    public RazorViewService(
      IActionContextAccessor actionContextAccessor,
      ITempDataProvider tempDataProvider,
      IRazorViewEngine razor)
    {
      _actionContext = actionContextAccessor.ActionContext
        ?? throw new InvalidOperationException("Failed to get the ActionContext.");

      _tempDataProvider = tempDataProvider;
      _razor = razor;
    }

    /// <summary>
    /// Render a Razor View to a string
    /// with no model
    /// </summary>
    /// <param name="viewName">The name of the view</param>
    /// <returns></returns>
    public async Task<(string view, ViewContext context)> RenderToString(string viewName)
      => await RenderToString<object>(viewName);

    /// <summary>
    /// Render a Razor View to a string, using the provided model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="viewName"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<(string view, ViewContext context)> RenderToString<T>(string viewName, T? model = null)
      where T : class
    {
      var view = FindView(viewName);

      await using var output = new StringWriter();

      var viewContext = new ViewContext(
          _actionContext,
          view,
          new ViewDataDictionary<T>(
              metadataProvider: new EmptyModelMetadataProvider(),
              modelState: new ModelStateDictionary())
          {
            Model = model
          },
          new TempDataDictionary(
              _actionContext.HttpContext,
              _tempDataProvider),
          output,
          new HtmlHelperOptions());

      await view.RenderAsync(viewContext);

      return (output.ToString(), viewContext);
    }

    /// <summary>
    /// <para>
    /// Tries to find a view taking into account the action context we are in
    /// and the culture specified on the request.
    /// </para>
    /// 
    /// <para>
    /// Starts with most granular culture (e.g. "en-GB") and works back up the hierarchy (e.g. "en")
    /// until finally trying no culture.
    /// </para>
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IView FindView(string viewName)
    {
      IEnumerable<string> searchedLocations = new List<string>();

      foreach(var viewPath in GetLocalisedViewPaths(viewName))
      {
        var getViewResult = _razor.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);
        if (getViewResult.Success)
        {
          return getViewResult.View;
        }

        var findViewResult = _razor.FindView(_actionContext, viewPath, isMainPage: true);
        if (findViewResult.Success)
        {
          return findViewResult.View;
        }

        searchedLocations = searchedLocations.Concat(
          getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations));
      }

      var errorMessage = string.Join(
          Environment.NewLine,
          new[] {
            $"Unable to find view '{viewName}'. The following locations were searched:"
          }
          .Concat(searchedLocations));

      throw new InvalidOperationException(errorMessage);
    }

    private List<string> GetLocalisedViewPaths(string viewPath)
    {
      List<string> paths = new();

      var pathSegments = viewPath.Split('/').ToList();

      var culture = _actionContext.HttpContext.Request.GetUICulture();

      do
      {
        // insert culture into the view path
        // e.g. for
        // Emails/MyEmail
        // return Emails/en-GB/MyEmail
        List<string> segments = new(pathSegments);
        segments.Insert(segments.Count - 1, culture.Name);

        paths.Add(Path.Combine(segments.ToArray()));

        // Continue up the Culture hierarchy
        culture = culture.Parent;
      } while (culture != culture.Parent);

      // Also add the uncultured path as a final fallback
      paths.Add(viewPath);

      return paths;
    }
  }
}
