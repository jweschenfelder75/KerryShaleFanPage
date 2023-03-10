using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Counter
    {
        private int currentCount = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected void IncrementCount()
        {
            currentCount++;
        }
    }
}
