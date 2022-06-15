namespace TodoLib.Models;

public class TodoItem
{
    public int Id { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public string Message { get; set; }


    public override string ToString()
    {
        return $"Id: {Id}; Date: {DateTime}; Message: {Message}";
    }
}