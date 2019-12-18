namespace silent_printing
{
    class Program
    {
        static void Main(string[] args)
        {
            new Print("EPSON TM-T82II Receipt", "Roll Paper 58 x 297 mm", 297, 0, args);
        }
    }
}
