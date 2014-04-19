namespace UniqueNamespace
{
    public static class Templates
    {
        private const string SqlTemplate = "{{FROM}} {{JOIN}} {{INNERJOIN}} {{LEFTJOIN}} {{RIGHTJOIN}} {{WHERE}} {{GROUPBY}} {{HAVING}} {{ORDERBY}}";

        public const string SelectionTemplate =
            "{{SELECT}} " + SqlTemplate;

        public const string CountTemplate =
            "SELECT Count(*) " + SqlTemplate;
    }
}