using Microsoft.JSInterop;

namespace Tnc.Games.TicTacToe.Web.Services;

public class ThemeService
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "ttt-theme";

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string> GetThemeAsync()
    {
        try
        {
            var theme = await _js.InvokeAsync<string>("window.tttTheme.get");
            return string.IsNullOrEmpty(theme) ? "light" : theme;
        }
        catch
        {
            return "light";
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        try
        {
            await _js.InvokeVoidAsync("window.tttTheme.set", theme);
            await _js.InvokeVoidAsync("window.tttTheme.setAttr", theme);
        }
        catch
        {
            // ignore
        }
    }
}
