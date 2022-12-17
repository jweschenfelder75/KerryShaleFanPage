using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class SurveyPrompt
    {
        // Demonstrates how a parent component can supply parameters
        [Parameter]
        public string? Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }
    }
}
