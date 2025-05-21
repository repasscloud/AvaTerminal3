namespace AvaTerminal3.Views
{
    public partial class InterimPage : ContentPage, IQueryAttributable
    {
        public string TaskType { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public InterimPage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            InitializeComponent();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("taskType"))
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                TaskType = query["taskType"]?.ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Use TaskType here
        }
    }
}
