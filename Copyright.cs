namespace SimpleKakeibo
{
    class Copyright
    {
        int firstYear;
        int currentYear;
        string author;

        public Copyright(int firstYear, int currentYear, string author)
        {
            this.firstYear = firstYear;
            this.currentYear = currentYear;
            this.author = author;
        }

        public string getText()
        {
            string output = "";
            output += "(c) " + firstYear;
            if (currentYear > firstYear)
            {
                if (currentYear - firstYear == 1)
                {
                    output += ",";
                } else
                {
                    output += "-";
                }
                output += currentYear;
            }
            output += " " + author + " all rights reserved.";
            return output;
        }
    }
}