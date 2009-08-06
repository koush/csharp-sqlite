//  $Header$

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using CS_SQLite3;
using System.Data;

public class Benchmark
{
  private static int nRecords = 100000;

  public static void Main()
  {
    runBenchmark();
    Console.WriteLine( "Enter to Continue: " );
    Console.ReadKey();
  }

  private static void runBenchmark()
  {
    SQLiteDatabase db;
    SQLiteVdbe stmt;
    SQLiteVdbe c1, c2;

    bool found;
    string s;

    string databaseName = "Benchmark.sqlite";
    Console.WriteLine( "\n\r" + databaseName );
    if ( File.Exists( databaseName ) ) File.Delete( databaseName );

    db = new SQLiteDatabase( databaseName );
    db.ExecuteNonQuery( "PRAGMA synchronous =  OFF" );
    db.ExecuteNonQuery( "PRAGMA temp_store =  MEMORY" );
    db.ExecuteNonQuery( "PRAGMA journal_mode = OFF" );
    db.ExecuteNonQuery( "PRAGMA locking_mode = EXCLUSIVE" );

    db.ExecuteNonQuery( "BEGIN" );
    db.ExecuteNonQuery( "create table TestIndex (i integer(8), s text)" );
    db.ExecuteNonQuery( "create index StrIndex on TestIndex (s)" );
    db.ExecuteNonQuery( "create index IntIndex on TestIndex (i)" );
    stmt = new SQLiteVdbe( db, "insert into TestIndex (i,s) values (?,?)" );
    DateTime start = DateTime.Now;
    long key = 1999;
    int i;
    for ( i = 0 ; i < nRecords ; i++ )
    {
      key = ( 3141592621L * key + 2718281829L ) % 1000000007L;
      stmt.Reset();
      stmt.BindLong( 1, key );
      stmt.BindText( 2, key.ToString() );
      stmt.ExecuteStep();
    }
    stmt.Close();
    db.ExecuteNonQuery( "COMMIT" );
    Console.WriteLine( "Elapsed time for inserting " + nRecords + " records: "
        + ( DateTime.Now - start ) + " milliseconds" );

    start = DateTime.Now;
    c1 = new SQLiteVdbe(db, "Select * from TestIndex where i=?" );
    c2 = new SQLiteVdbe( db, "Select * from TestIndex where s=?" );
    key = 1999;
    for ( i = 0 ; i < nRecords ; i++ )
    {
      key = ( 3141592621L * key + 2718281829L ) % 1000000007L;
      s = key.ToString();
      c1.Reset();
      c1.BindLong( 1, key );
      c1.ExecuteStep();

      c2.Reset();
      c2.BindText( 1, s );
      c2.ExecuteStep();
      found = c1.ResultColumnCount() == 2;
      Debug.Assert( found );
      Debug.Assert( c1.Result_Long(0) == key );
      Debug.Assert( c1.Result_Text(1) == ( s ) );

      found = c2.ResultColumnCount() == 2;
      Debug.Assert( found );
      Debug.Assert( (long)c2.Result_Long( 0 ) == key );
      Debug.Assert( (string)c2.Result_Text( 1 ) == ( s ) );

      int next = c1.ExecuteStep();
      Debug.Assert( next == csSQLite.SQLITE_DONE );

      next = c2.ExecuteStep();
      Debug.Assert( next == csSQLite.SQLITE_DONE );      
    }
    c1.Close();
    c2.Close();
    Console.WriteLine( "Elapsed time for performing " + nRecords * 2
         + " index searches: " + ( DateTime.Now - start )
         + " milliseconds" );

    start = DateTime.Now;
    key = Int64.MinValue;
    i = 0;
    c1 = new SQLiteVdbe( db, "Select * from TestIndex" );
    while ( c1.ExecuteStep() != csSQLite.SQLITE_DONE )
    {
      Debug.Assert( c1.Result_Long( 0 )  >= key );
      key = c1.Result_Long( 0 );
      i += 1;
    }
    c1.Close();
    Debug.Assert( i == nRecords );

    s = "";
    i = 0;
    c2 = new SQLiteVdbe( db, "Select * from TestIndex" );
    while ( c2.ExecuteStep() != csSQLite.SQLITE_DONE )
    {
      Debug.Assert( c2.Result_Text( 1).CompareTo( s ) >= 0 );
      s = c2.Result_Text( 1 );
      i += 1;
    }
    c2.Close();
    Debug.Assert( i == nRecords );
    Console.WriteLine( "Elapsed time for iterating through " + ( nRecords * 2 )
        + " records: " + ( DateTime.Now - start )
        + " milliseconds" );

    db.ExecuteNonQuery( "BEGIN" );
    start = DateTime.Now;
    key = 1999;
    stmt = new SQLiteVdbe( db, "delete from TestIndex where i=?" );
    for ( i = 0 ; i < nRecords ; i++ )
    {
      key = ( 3141592621L * key + 2718281829L ) % 1000000007L;
      stmt.Reset();
      stmt.BindLong( 1, key );
      stmt.ExecuteStep();
    }
    stmt.Close();
    db.ExecuteNonQuery( "COMMIT" );
    Console.WriteLine( "Elapsed time for deleting " + nRecords + " records: "
        + ( DateTime.Now - start ) + " milliseconds" );
    db.CloseDatabase();

  }
}
