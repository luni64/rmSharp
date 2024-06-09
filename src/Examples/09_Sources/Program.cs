using rmSharp;
using System.Diagnostics;

DB.sqLiteFile = "someDatabase.rmtree";

using (var db = new DB())
{

    // print all citations
    foreach (var repository in db.Repositories)
    {
        Trace.WriteLine(repository);
        foreach (var source in repository.Sources)
        {
            Trace.WriteLine($"   -{source}");
            foreach (var citation in source.Citations)
            {
                Trace.WriteLine($"    -{citation}");
            }
        }
    }

    // find all persons with more than 3 events and print corresponding citations
    var p = db.Persons.Where(p => p.Events.Count() > 3).FirstOrDefault();
    Trace.WriteLine($"{p}");
    foreach(var ev in p.Events.Where(e=>e.Citations.Count > 0))    
    {
        Trace.WriteLine($"  {ev}");
        foreach(var citation in ev.Citations)
        {
            Trace.WriteLine($"    Citation:  {citation} Source:{citation.Source}");
        }
    }
}






