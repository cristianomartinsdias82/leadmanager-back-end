//Source code url: https://www.youtube.com/watch?v=gW19LaAYczI ("Making async code run faster in C#")
using MediatR;

namespace Shared.Helpers;

public class TaskHelper
{
    public static async Task<IEnumerable<T>> WhenAll<T>(params Task<T>[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            return await allTasks;
        }
        catch (Exception)
        {
            
        }

        throw allTasks.Exception ?? throw new Exception("Erro durante a execução em paralelo das operações.");
    }

    public static async Task WhenAll(params Task[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            await allTasks;
            return;
        }
        catch (Exception)
        {

        }

        throw allTasks.Exception ?? throw new Exception("Erro durante a execução em paralelo das operações.");
    }
}