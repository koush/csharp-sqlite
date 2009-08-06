//  $Header$

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using CS_SQLite3;

public class Benchmark
{
  private static int nRecords = 1000;

  public static void Main()
  {
    runBenchmark();
    Console.WriteLine("Enter to Continue: ");
    Console.ReadKey();
  }

  private static void runBenchmark()
  {
    SQLiteDatabase db;
    //SQLiteCursor c1, c2;
    csSQLite.Vdbe stmt;

    bool found;
    string s;

    string databaseName = "Benchmark.sqlite";
    Console.WriteLine("\n\r" + databaseName);
    if (File.Exists(databaseName)) File.Delete(databaseName);

    db = new SQLiteDatabase(databaseName);
    db.ExecuteNonQuery("PRAGMA synchronous =  OFF");
    db.ExecuteNonQuery("PRAGMA temp_store =  MEMORY");
    db.ExecuteNonQuery("PRAGMA journal_mode = OFF");
    db.ExecuteNonQuery("PRAGMA locking_mode = EXCLUSIVE");

    db.ExecuteNonQuery("BEGIN");
    db.ExecuteNonQuery("create table TestIndex (i integer(8), s text)");
    db.ExecuteNonQuery("create index StrIndex on TestIndex (s)");
    db.ExecuteNonQuery("create index IntIndex on TestIndex (i)");
    stmt = db.CompileStatement("insert into TestIndex (i,s) values (?,?)");
    DateTime start = DateTime.Now;
    long key = 1999;
    int i;
    for (i = 0; i < nRecords; i++)
    {
      key = (3141592621L * key + 2718281829L) % 1000000007L;
      db.BindStatement(stmt, 1, csSQLite.SQLITE_INTEGER, key, null);
      db.BindStatement(stmt, 2, csSQLite.SQLITE_TEXT, 0, key.ToString());
      db.ExecuteStatement(stmt);
    }
    db.CloseStatement(stmt);
    db.ExecuteNonQuery("COMMIT");
    Console.WriteLine("Elapsed time for inserting " + nRecords + " records: "
        + (DateTime.Now - start) + " milliseconds");

    //start = DateTime.Now;
    //key = 1999;
    //for (i = 0; i < nRecords; i++)
    //{
    //  key = (3141592621L * key + 2718281829L) % 1000000007L;
    //  s = key.ToString();
    //  c1 = db.query("TestIndex", new String[] { "i", "s" }, "i=?", new String[] { s }, null, null, null);
    //  c2 = db.query("TestIndex", new String[] { "i", "s" }, "s=?", new String[] { s }, null, null, null);
    //  found = c1.moveToFirst();
    //  Debug.Assert(found);
    //  Debug.Assert(c1.getLong(1) == key);
    //  Debug.Assert(c1.getString(2) == (s));
    //  Debug.Assert(!c1.moveToNext());
    //  c1.close();

    //  found = c2.moveToFirst();
    //  Debug.Assert(found);
    //  Debug.Assert(c2.getLong(1) == key);
    //  Debug.Assert(c2.getString(2) == (s));
    //  Debug.Assert(!c2.moveToNext());
    //  c2.close();
    //}
    //Console.WriteLine("Elapsed time for performing " + nRecords * 2
    //    + " index searches: " + (DateTime.Now - start)
    //    + " milliseconds");

    //start = DateTime.Now;
    //key = Int64.MinValue;
    //i = 0;
    //c1 = db.query("TestIndex", new String[] { "i", "s" }, null, null, null, null, "i");
    //while (c1.moveToNext())
    //{
    //  Debug.Assert(c1.getLong(1) >= key);
    //  key = c1.getLong(1);
    //  i += 1;
    //}
    //c1.close();
    //Debug.Assert(i == nRecords);
    //s = "";
    //i = 0;
    //c2 = db.query("TestIndex", new String[] { "i", "s" }, null, null, null, null, "s");
    //while (c2.moveToNext())
    //{
    //  Debug.Assert(c2.getString(1).CompareTo(s) >= 0);
    //  s = c2.getString(1);
    //  i += 1;
    //}
    //Debug.Assert(i == nRecords);
    //Console.WriteLine("Elapsed time for iterating through " + (nRecords * 2)
    //    + " records: " + (DateTime.Now - start)
    //    + " milliseconds");

    db.ExecuteNonQuery("BEGIN");
    start = DateTime.Now;
    key = 1999;
    stmt = db.CompileStatement("delete from TestIndex where i=?");
    for (i = 0; i < nRecords; i++)
    {
      key = (3141592621L * key + 2718281829L) % 1000000007L;
      db.BindStatement(stmt, 1, csSQLite.SQLITE_INTEGER, key, null);
      db.ExecuteStatement(stmt);
    }
    db.CloseStatement(stmt);
    db.ExecuteNonQuery("COMMIT");
    Console.WriteLine("Elapsed time for deleting " + nRecords + " records: "
        + (DateTime.Now - start) + " milliseconds");
    db.CloseDatabase();

  }
}
