using Microsoft.JSInterop;

namespace EtayfeAdminPanel.Services
{
    public static class IJSExtentionMethods
    {

        public static async ValueTask<bool> confirMethod(this IJSRuntime js, string message)
        {
            bool confirm = await js.InvokeAsync<bool>("Confirm", message);
            return confirm;
        }
        public static ValueTask SaveAs(this IJSRuntime js, string fileName, byte[] content)
        {
            return js.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(content));
        }
        public static async ValueTask SetInLocalStorage(this IJSRuntime js, string key, string content)
          => await js.InvokeVoidAsync("localStorage.setItem",key, content );

        public static async ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
            => await js.InvokeAsync<string>("localStorage.getItem", key ); 

        public static async ValueTask RemoveItem(this IJSRuntime js, string key)
            => await js.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
