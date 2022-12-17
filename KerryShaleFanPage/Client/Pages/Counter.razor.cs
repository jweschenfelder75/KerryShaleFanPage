using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Counter
    {
        private int currentCount = 0;

        protected override async Task OnInitializedAsync()
        {
        }

        protected void IncrementCount()
        {
            currentCount++;
        }
    }
}
