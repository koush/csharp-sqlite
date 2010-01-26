//  $Header$
using System;
using System.Data;
using System.Collections;

namespace Community.Data.SQLite
{

  using sqlite = csSQLite.sqlite3;
  using Vdbe = csSQLite.Vdbe;
  /// <summary>
  /// C#-SQLite wrapper with functions for opening, closing and executing queries.
  /// </summary>
  public class SQLiteDatabase
  {
    // pointer to database
    private sqlite db;

    /// <summary>
    /// Creates new instance of SQLiteBase class with no database attached.
    /// </summary>
    public SQLiteDatabase()
    {
      db = null;
    }
    /// <summary>
    /// Creates new instance of SQLiteDatabase class and opens database with given name.
    /// </summary>
    /// <param name="DatabaseName">Name (and path) to SQLite database file</param>
    public SQLiteDatabase( String DatabaseName )
    {
      OpenDatabase( DatabaseName );
    }

    /// <summary>
    /// Opens database. 
    /// </summary>
    /// <param name="DatabaseName">Name of database file</param>
    public void OpenDatabase( String DatabaseName )
    {
      // opens database 
      if ( csSQLite.sqlite3_open( DatabaseName, ref db ) != csSQLite.SQLITE_OK )
      {
        // if there is some error, database pointer is set to 0 and exception is throws
        db = null;
        throw new Exception( "Error with opening database " + DatabaseName + "!" );
      }
    }

    /// <summary>
    /// Closes opened database.
    /// </summary>
    public void CloseDatabase()
    {
      // closes the database if there is one opened
      if ( db != null )
      {
        csSQLite.sqlite3_close( db );
      }
    }

    /// <summary>
    /// Returns connection
    /// </summary>
    public sqlite Connection()
    {
      return db;
    }

    /// <summary>
    /// Returns the list of tables in opened database.
    /// </summary>
    /// <returns></returns>
    public ArrayList GetTables()
    {
      // executes query that select names of all tables in master table of the database
      String query = "SELECT name FROM sqlite_master " +
                                  "WHERE type = 'table'" +
                                  "ORDER BY 1";
      DataTable table = ExecuteQuery( query );

      // Return all table names in the ArrayList
      ArrayList list = new ArrayList();
      foreach ( DataRow row in table.Rows )
      {
        list.Add( row.ItemArray[0].ToString() );
      }
      return list;
    }

    /// <summary>
    /// Executes query that does not return anything (e.g. UPDATE, INSERT, DELETE).
    /// </summary>
    /// <param name="query"></param>
    public void ExecuteNonQuery( String query )
    {
      // calles SQLite function that executes non-query
      csSQLite.sqlite3_exec( db, query, 0, 0, 0 );
      // if there is error, excetion is thrown
      if ( db.errCode != csSQLite.SQLITE_OK )
        throw new Exception( "Error with executing non-query: \"" + query + "\"!\n" + csSQLite.sqlite3_errmsg( db ) );
    }

    /// <summary>
    /// Executes query that does return something (e.g. SELECT).
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public DataTable ExecuteQuery( String query )
    {
      // compiled query
      SQLiteVdbe statement = new SQLiteVdbe(this, query);

      // table for result of query
      DataTable table = new DataTable();

      // create new instance of DataTable with name "resultTable"
      table = new DataTable( "resultTable" );

      // reads rows
      do { } while ( ReadNextRow( statement.VirtualMachine(), table ) == csSQLite.SQLITE_ROW );
      // finalize executing this query
      statement.Close();

      // returns table
      return table;
    }

    // private function for reading rows and creating table and columns
    private int ReadNextRow( Vdbe vm, DataTable table )
    {
      int columnCount = table.Columns.Count;
      if ( columnCount == 0 )
      {
        if ( ( columnCount = ReadColumnNames( vm, table ) ) == 0 ) return csSQLite.SQLITE_ERROR;
      }

      int resultType;
      if ( ( resultType = csSQLite.sqlite3_step( vm) ) == csSQLite.SQLITE_ROW )
      {
        object[] columnValues = new object[columnCount];

        for ( int i = 0 ; i < columnCount ; i++ )
        {
          int columnType = csSQLite.sqlite3_column_type( vm, i );
          switch ( columnType )
          {
            case csSQLite.SQLITE_INTEGER:
              {
                table.Columns[i].DataType = typeof(Int64);
                columnValues[i] = csSQLite.sqlite3_column_int(vm, i);
                break;
              }
            case csSQLite.SQLITE_FLOAT:
              {
                table.Columns[i].DataType = typeof(Double);
                columnValues[i] = csSQLite.sqlite3_column_double(vm, i);
                break;
              }
            case csSQLite.SQLITE_TEXT:
              {
                table.Columns[i].DataType = typeof(String);
                columnValues[i] = csSQLite.sqlite3_column_text(vm, i);
                break;
              }
            case csSQLite.SQLITE_BLOB:
              {
                table.Columns[i].DataType = typeof(Byte[]);
                columnValues[i] = csSQLite.sqlite3_column_blob(vm, i);
                break;
              }
            default:
              {
                table.Columns[i].DataType = null;
                columnValues[i] = "";
                break;
              }
          }
        }
        table.Rows.Add( columnValues );
      }
      return resultType;
    }
    // private function for creating Column Names
    // Return number of colums read
    private int ReadColumnNames( Vdbe vm, DataTable table )
    {

      String columnName = "";
      int columnType = 0;
      // returns number of columns returned by statement
      int columnCount = csSQLite.sqlite3_column_count( vm );
      object[] columnValues = new object[columnCount];

      try
      {
        // reads columns one by one
        for ( int i = 0 ; i < columnCount ; i++ )
        {
          columnName = csSQLite.sqlite3_column_name( vm, i );

          columnType = csSQLite.sqlite3_column_type( vm, i );

          switch ( columnType )
          {
            case csSQLite.SQLITE_INTEGER:
              {
                // adds new integer column to table
                table.Columns.Add( columnName, Type.GetType( "System.Int64" ) );
                break;
              }
            case csSQLite.SQLITE_FLOAT:
              {
                table.Columns.Add( columnName, Type.GetType( "System.Double" ) );
                break;
              }
            case csSQLite.SQLITE_TEXT:
              {
                table.Columns.Add( columnName, Type.GetType( "System.String" ) );
                break;
              }
            case csSQLite.SQLITE_BLOB:
              {
                table.Columns.Add( columnName, Type.GetType( "System.byte[]" ) );
                break;
              }
            default:
              {
                table.Columns.Add( columnName, Type.GetType( "System.String" ) );
                break;
              }
          }
        }
      }
      catch
      {
        return 0;
      }
      return table.Columns.Count;
    }

  }
}
