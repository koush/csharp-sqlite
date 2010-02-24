//  $Header$
using System;

namespace Community.CsharpSqlite
{

  using Vdbe = Sqlite3.Vdbe;

  /// <summary>
  /// C#-SQLite wrapper with functions for opening, closing and executing queries.
  /// </summary>
  public class SQLiteVdbe
  {
    private Vdbe vm = null;
    private string LastError = "";
    private int LastResult = 0;

    /// <summary>
    /// Creates new instance of SQLiteVdbe class by compiling a statement
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Vdbe</returns>
    public SQLiteVdbe( SQLiteDatabase db, String query )
    {
      vm = null;

      // prepare and compile 
      Sqlite3.PrepareV2NoTail( db.Connection(), query, query.Length, ref vm, 0 );
    }

    /// <summary>
    /// Return Virtual Machine Pointer
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Vdbe</returns>
    public Vdbe VirtualMachine()
    {
      return vm;
    }
    
    /// <summary>
    /// <summary>
    /// BindInteger
    /// </summary>
    /// <param name="index"></param>
    /// <param name="bInteger"></param>
    /// <returns>LastResult</returns>
    public int BindInteger(int index, int bInteger )
    {
      if ( (LastResult = Sqlite3.BindInt( vm, index, bInteger ))== Sqlite3.SQLITE_OK )
      { LastError = ""; }
      else
      {
        LastError = "Error " + LastError + "binding Integer [" + bInteger + "]";
      }
      return LastResult;
    }

    /// <summary>
    /// <summary>
    /// BindLong
    /// </summary>
    /// <param name="index"></param>
    /// <param name="bLong"></param>
    /// <returns>LastResult</returns>
    public int BindLong( int index, long bLong )
    {
      if ( ( LastResult = Sqlite3.BindInt64( vm, index, bLong ) ) == Sqlite3.SQLITE_OK )
      { LastError = ""; }
      else
      {
        LastError = "Error " + LastError + "binding Long [" + bLong + "]";
      }
      return LastResult;
    }

    /// <summary>
    /// BindText
    /// </summary>
    /// <param name="index"></param>
    /// <param name="bLong"></param>
    /// <returns>LastResult</returns>
    public int BindText(  int index, string bText )
    {
      if ( ( LastResult = Sqlite3.BindText( vm, index, bText ,-1,null) ) == Sqlite3.SQLITE_OK )
      { LastError = ""; }
      else
      {
        LastError = "Error " + LastError + "binding Text [" + bText + "]";
      }
      return LastResult;
    }

    /// <summary>
    /// Execute statement
    /// </summary>
    /// </param>
    /// <returns>LastResult</returns>
    public int ExecuteStep(   )
    {
      // Execute the statement
      int LastResult = Sqlite3.Step( vm );

      return LastResult;
    }

    /// <summary>
    /// Returns Result column as Long
    /// </summary>
    /// </param>
    /// <returns>Result column</returns>
    public long Result_Long(int index)
    {
      return Sqlite3.ColumnInt64( vm, index );
    }

    /// <summary>
    /// Returns Result column as Text
    /// </summary>
    /// </param>
    /// <returns>Result column</returns>
    public string Result_Text( int index )
    {
      return Sqlite3.ColumnText( vm, index );
    }

    
    /// <summary>
    /// Returns Count of Result Rows
    /// </summary>
    /// </param>
    /// <returns>Count of Results</returns>
    public int ResultColumnCount( )
    {
      return vm.pResultSet == null ? 0 : vm.pResultSet.Length;
    }

    /// <summary>
    /// Reset statement
    /// </summary>
    /// </param>
    /// </returns>
    public void Reset()
    {
      // Reset the statment so it's ready to use again
      Sqlite3.Reset( vm );
    }
    
    /// <summary>
    /// Closes statement
    /// </summary>
    /// </param>
    /// <returns>LastResult</returns>
    public void Close()
    {
      Sqlite3.Finalize( ref vm );
    }
  
  }
}
