namespace LokerMVC.Models;

public class RespondQuizView
{
    public int HeaderId { get; set; }
    public List<DataQuiz> DataQuiz { get; set; } = new();
}

public class DataQuiz
{
    public int DetailJawabanId { get; set; }
    public int No { get; set; }
    public string Pertanyaan { get; set; }
    public string Jawaban { get; set; }
    public string[] JawabanCollection { get; set; }
    public string BentukIsian { get; set; }
    public bool IsRequired { get; set; }
    public List<PilihanJawaban> PilihanJawaban { get; set; } = new();
}

public class PilihanJawaban
{
    public string Keterangan { get; set; }
}