using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Contact
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        protected BrowserService BrowserService { get; set; }

        private ContactDataDto _contact = new ContactDataDto();

        private IList<string> _categories => GetCategories();

        private string _captchaResponse = string.Empty;

        private bool _sendMailSuccessResponse = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _contact.Category = _categories.FirstOrDefault() ?? string.Empty;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeAsync<int>("googleRecaptcha", DotNetObjectReference.Create(this), "google_recaptcha ", "6Ldx5v0kAAAAABzCXkUqTXKRYSd1Wx3dwiUW-onm");  // TODO: Make configurable!
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnSuccess(string response)
        {
            _captchaResponse = response;
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnExpired(string response)
        {
            //...
        }

        private IList<string> GetCategories()
        {
            return new List<string>() { 
                Translate["Normal request"], 
                Translate["I represent a company"],
                Translate["I represent Kerry Shale (e.g. agent)"] 
            };
        }

        private async Task SubmitAsync(ContactDataDto args)
        {
            if (string.IsNullOrWhiteSpace(_captchaResponse))
            {
                return;
            }

            try
            {
                var contactData = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json");
                var data = await Http.PostAsync("webapi/EMail", contactData);
                _sendMailSuccessResponse = data.IsSuccessStatusCode;
                StateHasChanged();
            } 
            catch (Exception ex) 
            {
                var exception = ex;
            }
        }

        private Task InvalidSubmitAsync()
        {
            //...
            return Task.CompletedTask;
        }
    }
}
