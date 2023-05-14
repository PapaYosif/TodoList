namespace Tester.Model
{
    public class aTask
    {
        private string title, description;
        private bool isDone;
        public string Title
        {
            get => title;

            set => title = value;

        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public bool IsDone
        {
            get => isDone;
            set => isDone = value;
        }


    }
}
