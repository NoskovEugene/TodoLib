using System.Reflection;
using System.Text;
using Serilog;
using Terminal.SharedModels.Attributes.UtilityAttributes;
using TodoLib.Models;
using UI.RequestService;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace TodoLib;

[Utility("todo")]
public class TodoController // Утилита
{
    #region constructor

    private ILogger _logger;

    private List<TodoItem> _todoItems;


    public TodoController(ILogger logger)
    {
        _logger = logger;
        _todoItems = new();
    }

    #endregion

    [Command("test")]
    public void Test()
    {
        var assembly = Assembly.GetExecutingAssembly();
        _logger.Information(Directory.GetCurrentDirectory());
        _logger.Information(assembly.Location);
    } // Команда test

    [Command("add")]
    public void Add(DateTime dateTime, string message)
    {
        _todoItems.Add(new TodoItem()
        {
            Id = _todoItems.Count + 1,
            Message = message,
            DateTime = dateTime
        });
        _logger.Information($"Item added. Id = {_todoItems.Count}");
    } // команда add

    [Command("find")]
    public void Find(int id)
    {
        var item = _todoItems.FirstOrDefault(x => x.Id == id);
        _logger.Information(item != null ? item.ToString() : "Todo item was not found");
    } // команда 

    [Command("showjson")]
    public void ShowJson()
    {
        var json = JsonConvert.SerializeObject(_todoItems);
        _logger.Information(json);
    }

    [Command("remove")]
    public void Remove(int id)
    {
        var item = _todoItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            _logger.Information("Item was not found");
            return;
        }

        _todoItems.Remove(item);
        _logger.Information("Item was remove");
    }

    [Command("list")]
    public void List()
    {
        _logger.Information("---Todo items---");
        _todoItems.ForEach(x => { _logger.Information(x.ToString()); });
        _logger.Information("---end---");
    }

    [Command("savejson")]
    public void Save(string fileName, DateTime dateTime)
    {
        var collection = _todoItems.Where(x => x.DateTime == dateTime).ToList();
        SaveToFile(fileName, collection);
        _logger.Information("Collection saved");
    }

    [Command("savejson")]
    public void Save(string fileName)
    {
        SaveToFile(fileName, _todoItems);
        _logger.Information("Collection saved");
    }

    [Command("loadjson")]
    public void Load(string pathToFile)
    {
        if (File.Exists(pathToFile))
        {
            var json = File.ReadAllText(pathToFile);
            _logger.Information("Trying parse json");
            try
            {
                _todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(json);
                _logger.Information("Collection loaded");
            }
            catch (Exception e)
            {
                _logger.Error("Cannot convert json to todo collection");
            }
        }
        else
        {
            _logger.Information("File not found");
        }
    }

    private void SaveToFile(string fileName, List<TodoItem> collection)
    {
        var json = JsonConvert.SerializeObject(collection);
        if (File.Exists(fileName))
        {
            _logger.Information("File exist.");
            using var writer = new StreamWriter(fileName, false);
            writer.Write(json);
        }
        else
        {
            using var fileOpen = new FileStream(fileName, FileMode.Create);
            using var writer = new StreamWriter(fileOpen);
            writer.Write(json);
        }
    }
}