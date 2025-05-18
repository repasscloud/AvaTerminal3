namespace AvaTerminal3.Views
{
    public partial class InterimPage : ContentPage, IQueryAttributable
    {
        public string TaskType { get; set; }

        public InterimPage()
        {
            InitializeComponent();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("taskType"))
            {
                TaskType = query["taskType"]?.ToString();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Use TaskType here
        }
    }
}
