using System.Diagnostics;

using i64 = System.Int64;
using u32 = System.UInt32;
using u64 = System.UInt64;

using Pgno = System.UInt32;


namespace CS_SQLite3
{
#if !NO_TCL
  using tcl.lang;
  using Tcl_Interp = tcl.lang.Interp;
  using Tcl_CmdInfo = tcl.lang.Command;
  using Tcl_CmdProc = tcl.lang.Interp.dxObjCmdProc;
  using System.Text;

  public partial class csSQLite
  {
    /*
    ** 2001 September 15
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** Code for testing the btree.c module in SQLite.  This code
    ** is not included in the SQLite library.  It is used for automated
    ** testing of the SQLite library.
    **
    ** $Id: test3.c,v 1.104 2009/05/04 11:42:30 danielk1977 Exp $
    **
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library 
    **
    **  $Header$
    *************************************************************************
    */
    //#include "sqliteInt.h"
    //#include "btreeInt.h"
    //#include "tcl.h"
    //#include <stdlib.h>
    //#include <string.h>

    /*
    ** Interpret an SQLite error number
    */
    static string errorName( int rc )
    {
      string zName;
      switch ( rc )
      {
        case SQLITE_OK: zName = "SQLITE_OK"; break;
        case SQLITE_ERROR: zName = "SQLITE_ERROR"; break;
        case SQLITE_PERM: zName = "SQLITE_PERM"; break;
        case SQLITE_ABORT: zName = "SQLITE_ABORT"; break;
        case SQLITE_BUSY: zName = "SQLITE_BUSY"; break;
        case SQLITE_NOMEM: zName = "SQLITE_NOMEM"; break;
        case SQLITE_READONLY: zName = "SQLITE_READONLY"; break;
        case SQLITE_INTERRUPT: zName = "SQLITE_INTERRUPT"; break;
        case SQLITE_IOERR: zName = "SQLITE_IOERR"; break;
        case SQLITE_CORRUPT: zName = "SQLITE_CORRUPT"; break;
        case SQLITE_FULL: zName = "SQLITE_FULL"; break;
        case SQLITE_CANTOPEN: zName = "SQLITE_CANTOPEN"; break;
        case SQLITE_PROTOCOL: zName = "SQLITE_PROTOCOL"; break;
        case SQLITE_EMPTY: zName = "SQLITE_EMPTY"; break;
        case SQLITE_LOCKED: zName = "SQLITE_LOCKED"; break;
        default: zName = "SQLITE_Unknown"; break;
      }
      return zName;
    }

    /*
    ** A bogus sqlite3 connection structure for use in the btree
    ** tests.
    */
    static sqlite3 sDb = new sqlite3();
    static int nRefSqlite3 = 0;

    /*
    ** Usage:   btree_open FILENAME NCACHE FLAGS
    **
    ** Open a new database
    */
    static int btree_open(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt = null;
      int rc; int nCache = 0; int flags = 0;
      string zBuf = "";
      if ( argc != 4 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " FILENAME NCACHE FLAGS\"", "" );
        return TCL.TCL_ERROR;
      }
      if ( TCL.Tcl_GetInt( interp, argv[2], ref nCache ) ) return TCL.TCL_ERROR;
      if ( TCL.Tcl_GetInt( interp, argv[3], ref flags ) ) return TCL.TCL_ERROR;
      nRefSqlite3++;
      if ( nRefSqlite3 == 1 )
      {
        sDb.pVfs = sqlite3_vfs_find( null );
        sDb.mutex = sqlite3MutexAlloc( SQLITE_MUTEX_RECURSIVE );
        sqlite3_mutex_enter( sDb.mutex );
      }
      rc = sqlite3BtreeOpen( argv[1].ToString(), sDb, ref pBt, flags,
      SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE | SQLITE_OPEN_MAIN_DB );
      if ( rc != SQLITE_OK )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null );
        return TCL.TCL_ERROR;
      }
      sqlite3BtreeSetCacheSize( pBt, nCache );
      sqlite3_snprintf( 100, ref zBuf, "->%p", pBt );
      if ( TCL.Tcl_CreateCommandPointer( interp, zBuf, pBt ) )
      {
        return TCL.TCL_ERROR;
      }
      else
        TCL.Tcl_AppendResult( interp, zBuf, null );
      return TCL.TCL_OK;
    }

    /*
    ** Usage:   btree_close ID
    **
    ** Close the given database.
    */
    static int btree_close(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int rc;
      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"", null );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
      rc = sqlite3BtreeClose( ref pBt );
      if ( rc != SQLITE_OK )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null );
        return TCL.TCL_ERROR;
      }
      nRefSqlite3--;
      if ( nRefSqlite3 == 0 )
      {
        sqlite3_mutex_leave( sDb.mutex );
        sqlite3_mutex_free( ref sDb.mutex );
        sDb.mutex = null;
        sDb.pVfs = null;
      }
      return TCL.TCL_OK;
    }


    /*
    ** Usage:   btree_begin_transaction ID
    **
    ** Start a new transaction
    */
    static int btree_begin_transaction(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int rc;
      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"", null );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      rc = sqlite3BtreeBeginTrans( pBt, 1 );
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      if ( rc != SQLITE_OK )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      return TCL.TCL_OK;
    }

    /*
    ** Usage:   btree_rollback ID
    **
    ** Rollback changes
    */
    static int btree_rollback(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int rc;
      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"", null );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      rc = sqlite3BtreeRollback( pBt );
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      if ( rc != SQLITE_OK )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      return TCL.TCL_OK;
    }

    /*
    ** Usage:   btree_commit ID
    **
    ** Commit all changes
    */
    static int btree_commit(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int rc;
      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"", null );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      rc = sqlite3BtreeCommit( pBt );
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      if ( rc != SQLITE_OK )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      return TCL.TCL_OK;
    }

    /*
    ** Usage:   btree_begin_statement ID
    **
    ** Start a new statement transaction
    */
    //static int btree_begin_statement(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc;
    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //  rc = sqlite3BtreeBeginStmt(pBt,1);
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_rollback_statement ID
    **
    ** Rollback changes
    */
    //static int btree_rollback_statement(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc;
    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //rc = sqlite3BtreeSavepoint(pBt, SAVEPOINT_ROLLBACK, 0);
    //if( rc==SQLITE_OK ){
    //  rc = sqlite3BtreeSavepoint(pBt, SAVEPOINT_RELEASE, 0);
    //}
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_commit_statement ID
    **
    ** Commit all changes
    */
    //static int btree_commit_statement(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc;
    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //    rc = sqlite3BtreeSavepoint(pBt, SAVEPOINT_RELEASE, 0);
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_create_table ID FLAGS
    **
    ** Create a new table in the database
    */
    //static int btree_create_table(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc, iTable, flags;
    //  char zBuf[30];
    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID FLAGS\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], flags) ) return TCL.TCL_ERROR;
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //  rc = sqlite3BtreeCreateTable(pBt, iTable, flags);
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  sqlite3_snprintf(100, ref zBuf, "%d", iTable);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_drop_table ID TABLENUM
    **
    ** Delete an entire table from the database
    */
    //static int btree_drop_table(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int iTable;
    //  int rc;
    //  int notUsed1;
    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID TABLENUM\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], iTable) ) return TCL.TCL_ERROR;
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //  rc = sqlite3BtreeDropTable(pBt, iTable, notUsed1);
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_clear_table ID TABLENUM
    **
    ** Remove all entries from the given table but keep the table around.
    */
    //static int btree_clear_table(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int iTable;
    //  int rc;
    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID TABLENUM\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], iTable) ) return TCL.TCL_ERROR;
#if SQLITE_TEST
    //  sqlite3BtreeEnter(pBt);
#endif
    //  rc = sqlite3BtreeClearTable(pBt, iTable,0);
#if SQLITE_TEST
    //  sqlite3BtreeLeave(pBt);
#endif
    //  if( rc!=SQLITE_OK ){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_get_meta ID
    **
    ** Return meta data
    */
    //static int btree_get_meta(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc;
    //  int i;
    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  for(i=0; i<SQLITE_N_BTREE_META; i++){
    //    char zBuf[30];
    //    u32 v;
#if SQLITE_TEST
    //    sqlite3BtreeEnter(pBt);
#endif
    //    rc = sqlite3BtreeGetMeta(pBt, i, v);
#if SQLITE_TEST
    //    sqlite3BtreeLeave(pBt);
#endif
    //    if( rc!=SQLITE_OK ){
    //     TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //      return TCL.TCL_ERROR;
    //    }
    //    sqlite3_snprintf(100, ref zBuf,"%d",v);
    //   TCL.Tcl_AppendElement(interp, zBuf);
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_update_meta ID METADATA...
    **
    ** Return meta data
    */
    //static int btree_update_meta(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int rc;
    //  int i;
    //  int aMeta[SQLITE_N_BTREE_META];

    //  if( argc!=2+SQLITE_N_BTREE_META ){
    //    char zBuf[30];
    //    sqlite3_snprintf(100, ref zBuf,"%d",SQLITE_N_BTREE_META);
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID METADATA...\" (METADATA is ", zBuf, " integers)", 0);
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  for(i=1; i<SQLITE_N_BTREE_META; i++){
    //    if(TCL.Tcl_GetInt(interp, argv[i+2], aMeta[i]) ) return TCL.TCL_ERROR;
    //  }
    //  for(i=1; i<SQLITE_N_BTREE_META; i++){
#if SQLITE_TEST
    //    sqlite3BtreeEnter(pBt);
#endif
    //    rc = sqlite3BtreeUpdateMeta(pBt, i, aMeta[i]);
#if SQLITE_TEST
    //    sqlite3BtreeLeave(pBt);
#endif
    //    if( rc!=SQLITE_OK ){
    //     TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //      return TCL.TCL_ERROR;
    //    }
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_pager_stats ID
    **
    ** Returns pager statistics
    */
    static int btree_pager_stats(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int i;
      int[] a;

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );

      /* Normally in this file, with a b-tree handle opened using the 
      ** [btree_open] command it is safe to call sqlite3BtreeEnter() directly.
      ** But this function is sometimes called with a btree handle obtained
      ** from an open SQLite connection (using [btree_from_db]). In this case
      ** we need to obtain the mutex for the controlling SQLite handle before
      ** it is safe to call sqlite3BtreeEnter().
      */
      sqlite3_mutex_enter( pBt.db.mutex );

#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      a = sqlite3PagerStats( sqlite3BtreePager( pBt ) );
      for ( i = 0 ; i < 11 ; i++ )
      {
        string[] zName = new string[]{
"ref", "page", "max", "size", "state", "err",
"hit", "miss", "ovfl", "read", "write"
};
        string zBuf = "";//char zBuf[100];
        TCL.Tcl_AppendElement( interp, zName[i] );
        sqlite3_snprintf( 100, ref zBuf, "%d", a[i] );
        TCL.Tcl_AppendElement( interp, zBuf );
      }
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      /* Release the mutex on the SQLite handle that controls this b-tree */
      sqlite3_mutex_leave( pBt.db.mutex );
      return TCL.TCL_OK;
    }

    /*
    ** Usage:   btree_integrity_check ID ROOT ...
    **
    ** Look through every page of the given BTree file to verify correct
    ** formatting and linkage.  Return a line of text for each problem found.
    ** Return an empty string if everything worked.
    */
    //static int btree_integrity_check(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  int nRoot;
    //  int *aRoot;
    //  int i;
    //  int nErr;
    //  char *zResult;

    //  if( argc<3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID ROOT ...\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  nRoot = argc-2;
    //  aRoot = (int*)sqlite3Malloc( sizeof(int)*(argc-2) );
    //  for(i=0; i<argc-2; i++){
    //    if(TCL.Tcl_GetInt(interp, argv[i+2], aRoot[i]) ) return TCL.TCL_ERROR;
    //  }
#if !SQLITE_OMIT_INTEGRITY_CHECK
    //  sqlite3BtreeEnter(pBt);
    //  zResult = sqlite3BtreeIntegrityCheck(pBt, aRoot, nRoot, 10000, nErr);
    //  sqlite3BtreeLeave(pBt);
#else
//  zResult = 0;
#endif
    //  sqlite3DbFree(db,(void*)aRoot);
    //  if( zResult ){
    //   TCL.Tcl_AppendResult(interp, zResult);
    //    sqlite3DbFree(db,zResult); 
    //  }
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_cursor_list ID
    **
    ** Print information about all cursors to standard output for debugging.
    */
    //static int btree_cursor_list(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pBt);
    //  sqlite3BtreeCursorList(pBt);
    //  sqlite3BtreeLeave(pBt);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_cursor ID TABLENUM WRITEABLE
    **
    ** Create a new cursor.  Return the ID for the cursor.
    */
    static int btree_cursor(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int iTable = 0;
      BtCursor pCur;
      int rc;
      int wrFlag = 0;
      string zBuf = "";//char zBuf[30];

      if ( argc != 4 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID TABLENUM WRITEABLE\"" );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
      if ( TCL.Tcl_GetInt( interp, argv[2], ref  iTable ) ) return TCL.TCL_ERROR;
      if ( TCL.Tcl_GetBoolean( interp, argv[3], ref wrFlag ) ) return TCL.TCL_ERROR;
      //pCur = (BtCursor )ckalloc(sqlite3BtreeCursorSize());
      pCur = new BtCursor();// memset( pCur, 0, sqlite3BtreeCursorSize() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      rc = sqlite3BtreeCursor( pBt, iTable, wrFlag, null, pCur );
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      if ( rc != 0 )
      {
        pCur = null;// ckfree( pCur );
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      sqlite3_snprintf( 30, ref  zBuf, "->%p", pCur );
      if ( TCL.Tcl_CreateCommandPointer( interp, zBuf, pCur ) )
      {
        return TCL.TCL_ERROR;
      }
      else
        TCL.Tcl_AppendResult( interp, zBuf );
      return SQLITE_OK;
    }

    /*
    ** Usage:   btree_close_cursor ID
    **
    ** Close a cursor opened using btree_cursor.
    */
    static int btree_close_cursor(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      BtCursor pCur;
      Btree pBt;
      int rc;

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pCur = (BtCursor)sqlite3TestTextToPtr( interp, argv[1].ToString() );
      pBt = pCur.pBtree;
#if SQLITE_TEST
      sqlite3BtreeEnter( pBt );
#endif
      rc = sqlite3BtreeCloseCursor( pCur );
#if SQLITE_TEST
      sqlite3BtreeLeave( pBt );
#endif
      pCur = null;//ckfree( (char*)pCur );
      if ( rc != 0 )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      return SQLITE_OK;
    }

    /*
    ** Usage:   btree_move_to ID KEY
    **
    ** Move the cursor to the entry with the given key.
    */
    //static int btree_move_to(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  int res;
    //  char zBuf[20];

    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID KEY\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  if( sqlite3BtreeFlags(pCur) & BTREE_INTKEY ){
    //    int iKey;
    //    if(TCL.Tcl_GetInt(interp, argv[2], iKey) ){
    //      sqlite3BtreeLeave(pCur.pBtree);
    //      return TCL.TCL_ERROR;
    //    }
    //    rc = sqlite3BtreeMovetoUnpacked(pCur, 0, iKey, 0, res);
    //  }else{
    //    rc = sqlite3BtreeMoveto(pCur, argv[2], strlen(argv[2]), 0, res);  
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  if( res<0 ) res = -1;
    //  if( res>0 ) res = 1;
    //  sqlite3_snprintf(100, ref zBuf,"%d",res);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_delete ID
    **
    ** Delete the entry that the cursor is pointing to
    */
    //static int btree_delete(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  rc = sqlite3BtreeDelete(pCur);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_insert ID KEY DATA ?NZERO?
    **
    ** Create a new entry with the given key and data.  If an entry already
    ** exists with the same key the old entry is overwritten.
    */
    //static int btree_insert(
    //  object clientdata,
    // Tcl_Interp interp,
    //  int objc,
    // Tcl_Obj[] objv
    //){
    //  BtCursor pCur;
    //  int rc;
    //  int nZero;

    //  if( objc!=4 && objc!=5 ){
    //   TCL.Tcl_WrongNumArgs(interp, 1, objv, "ID KEY DATA ?NZERO?");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(Tcl_GetString(objv[1]));
    //  if( objc==5 ){
    //    if(TCL.Tcl_GetIntFromObj(interp, objv[4], nZero) ) return TCL.TCL_ERROR;
    //  }else{
    //    nZero = 0;
    //  }
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  if( sqlite3BtreeFlags(pCur) & BTREE_INTKEY ){
    //    i64 iKey;
    //    int len;
    //    unsigned char pBuf;
    //    if(TCL.Tcl_GetWideIntFromObj(interp, objv[2], iKey) ){
    //      sqlite3BtreeLeave(pCur.pBtree);
    //      return TCL.TCL_ERROR;
    //    }
    //    pBuf =TCL.Tcl_GetByteArrayFromObj(objv[3], len);
    //    rc = sqlite3BtreeInsert(pCur, 0, iKey, pBuf, len, nZero, 0,0);
    //  }else{
    //    int keylen;
    //    int dlen;
    //    unsigned char pKBuf;
    //    unsigned char pDBuf;
    //    pKBuf =TCL.Tcl_GetByteArrayFromObj(objv[2], keylen);
    //    pDBuf =TCL.Tcl_GetByteArrayFromObj(objv[3], dlen);
    //    rc = sqlite3BtreeInsert(pCur, pKBuf, keylen, pDBuf, dlen, nZero, 0,0);
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_next ID
    **
    ** Move the cursor to the next entry in the table.  Return 0 on success
    ** or 1 if the cursor was already on the last entry in the table or if
    ** the table is empty.
    */
    static int btree_next(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      BtCursor pCur;
      int rc;
      int res = 0;
      string zBuf = "";//char zBuf[100];

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pCur = (BtCursor)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pCur.pBtree );
#endif
      rc = sqlite3BtreeNext( pCur, ref res );
#if SQLITE_TEST
      sqlite3BtreeLeave( pCur.pBtree );
#endif
      if ( rc != 0 )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      sqlite3_snprintf( 100, ref zBuf, "%d", res );
      TCL.Tcl_AppendResult( interp, zBuf );
      return SQLITE_OK;
    }

    /*
    ** Usage:   btree_prev ID
    **
    ** Move the cursor to the previous entry in the table.  Return 0 on
    ** success and 1 if the cursor was already on the first entry in
    ** the table or if the table was empty.
    */
    //static int btree_prev(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  int res = 0;
    //  string zBuf="";//char zBuf[100];

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  rc = sqlite3BtreePrevious(pCur, res);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  sqlite3_snprintf(100, ref zBuf,"%d",res);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_first ID
    **
    ** Move the cursor to the first entry in the table.  Return 0 if the
    ** cursor was left point to something and 1 if the table is empty.
    */
    static int btree_first(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      BtCursor pCur;
      int rc;
      int res = 0;
      string zBuf = "";//[100];

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pCur = (BtCursor)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pCur.pBtree );
#endif
      rc = sqlite3BtreeFirst( pCur, ref res );
#if SQLITE_TEST
      sqlite3BtreeLeave( pCur.pBtree );
#endif
      if ( rc != 0 )
      {
        TCL.Tcl_AppendResult( interp, errorName( rc ), null ); ;
        return TCL.TCL_ERROR;
      }
      sqlite3_snprintf( 100, ref zBuf, "%d", res );
      TCL.Tcl_AppendResult( interp, zBuf );
      return SQLITE_OK;
    }

    /*
    ** Usage:   btree_last ID
    **
    ** Move the cursor to the last entry in the table.  Return 0 if the
    ** cursor was left point to something and 1 if the table is empty.
    */
    //static int btree_last(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  int res = 0;
    //  string zBuf="";//char zBuf[100];

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  rc = sqlite3BtreeLast(pCur, res);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    return TCL.TCL_ERROR;
    //  }
    //  sqlite3_snprintf(100, ref zBuf,"%d",res);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_eof ID
    **
    ** Return TRUE if the given cursor is not pointing at a valid entry.
    ** Return FALSE if the cursor does point to a valid entry.
    */
    //static int btree_eof(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  char zBuf[50];

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  rc = sqlite3BtreeEof(pCur);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  sqlite3_snprintf(100, ref zBuf, "%d", rc);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_keysize ID
    **
    ** Return the number of bytes of key.  For an INTKEY table, this
    ** returns the key itself.
    */
    //static int btree_keysize(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  u64 n;
    //  char zBuf[50];

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  sqlite3BtreeKeySize(pCur, (i64*)&n);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  sqlite3_snprintf(100, ref zBuf, "%llu", n);
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_key ID
    **
    ** Return the key for the entry at which the cursor is pointing.
    */
    //static int btree_key(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  u64 n;
    //  char *zBuf;

    //  if( argc!=2 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  sqlite3BtreeKeySize(pCur, (i64*)&n);
    //  if( sqlite3BtreeFlags(pCur) & BTREE_INTKEY ){
    //    char zBuf2[60];
    //    sqlite3_snprintf(sizeof(zBuf2),zBuf2, "%llu", n);
    //   TCL.Tcl_AppendResult(interp, zBuf2);
    //  }else{
    //    zBuf = sqlite3Malloc( n+1 );
    //    rc = sqlite3BtreeKey(pCur, 0, n, zBuf);
    //    if( rc !=0){
    //      sqlite3BtreeLeave(pCur.pBtree);
    //     TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //      return TCL.TCL_ERROR;
    //    }
    //    zBuf[n] = 0;
    //   TCL.Tcl_AppendResult(interp, zBuf);
    //    sqlite3DbFree(db,zBuf);
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_data ID ?N?
    **
    ** Return the data for the entry at which the cursor is pointing.
    */
    //static int btree_data(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  u32 n;
    //  char *zBuf;

    //  if( argc!=2 && argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  if( argc==2 ){
    //    sqlite3BtreeDataSize(pCur, n);
    //  }else{
    //    n = atoi(argv[2]);
    //  }
    //  zBuf = sqlite3Malloc( n+1 );
    //  rc = sqlite3BtreeData(pCur, 0, n, zBuf);
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    sqlite3DbFree(db,zBuf);
    //    return TCL.TCL_ERROR;
    //  }
    //  zBuf[n] = 0;
    // TCL.Tcl_AppendResult(interp, zBuf);
    //  sqlite3DbFree(db,zBuf);
    //  return SQLITE_OK;
    //}

    /*
    ** Usage:   btree_fetch_key ID AMT
    **
    ** Use the sqlite3BtreeKeyFetch() routine to get AMT bytes of the key.
    ** If sqlite3BtreeKeyFetch() fails, return an empty string.
    */
    //static int btree_fetch_key(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int n;
    //  int amt;
    //  u64 nKey;
    //  const char *zBuf;
    //  char zStatic[1000];

    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID AMT\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], n) ) return TCL.TCL_ERROR;
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  sqlite3BtreeKeySize(pCur, (i64*)&nKey);
    //  zBuf = sqlite3BtreeKeyFetch(pCur, amt);
    //  if( zBuf && amt>=n ){
    //    Debug.Assert( nKey<sizeof(zStatic) );
    //    if( n>0 ) nKey = n;
    //    memcpy(zStatic, zBuf, (int)nKey); 
    //    zStatic[nKey] = 0;
    //   TCL.Tcl_AppendResult(interp, zStatic);
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_fetch_data ID AMT
    **
    ** Use the sqlite3BtreeDataFetch() routine to get AMT bytes of the key.
    ** If sqlite3BtreeDataFetch() fails, return an empty string.
    */
    //static int btree_fetch_data(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int n;
    //  int amt;
    //  u32 nData;
    //  const char *zBuf;
    //  char zStatic[1000];

    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID AMT\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], n) ) return TCL.TCL_ERROR;
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  sqlite3BtreeDataSize(pCur, nData);
    //  zBuf = sqlite3BtreeDataFetch(pCur, amt);
    //  if( zBuf && amt>=n ){
    //    Debug.Assert( nData<sizeof(zStatic) );
    //    if( n>0 ) nData = n;
    //    memcpy(zStatic, zBuf, (int)nData); 
    //    zStatic[nData] = 0;
    //   TCL.Tcl_AppendResult(interp, zStatic);
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    //  return TCL.TCL_OK;
    //}

    /*
    ** Usage:   btree_payload_size ID
    **
    ** Return the number of bytes of payload
    */
    static int btree_payload_size(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      BtCursor pCur;
      i64 n1 = 0;
      u32 n2 = 0;
      string zBuf = "";//[50];

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pCur = (BtCursor)sqlite3TestTextToPtr( interp, argv[1].ToString() );
#if SQLITE_TEST
      sqlite3BtreeEnter( pCur.pBtree );
#endif
      if ( ( sqlite3BtreeFlags( pCur ) & BTREE_INTKEY ) != 0 )
      {
        n1 = 0;
      }
      else
      {
        sqlite3BtreeKeySize( pCur, ref n1 );
      }
      sqlite3BtreeDataSize( pCur, ref n2 );
#if SQLITE_TEST
      sqlite3BtreeLeave( pCur.pBtree );
#endif
      sqlite3_snprintf( 30, ref zBuf, "%d", (int)( n1 + n2 ) );
      TCL.Tcl_AppendResult( interp, zBuf );
      return SQLITE_OK;
    }

    /*
    ** Usage:   btree_cursor_info ID ?UP-CNT?
    **
    ** Return integers containing information about the entry the
    ** cursor is pointing to:
    **
    **   aResult[0] =  The page number
    **   aResult[1] =  The entry number
    **   aResult[2] =  Total number of entries on this page
    **   aResult[3] =  Cell size (local payload + header)
    **   aResult[4] =  Number of free bytes on this page
    **   aResult[5] =  Number of free blocks on the page
    **   aResult[6] =  Total payload size (local + overflow)
    **   aResult[7] =  Header size in bytes
    **   aResult[8] =  Local payload size
    **   aResult[9] =  Parent page number
    **   aResult[10]=  Page number of the first overflow page
    */
    //static int btree_cursor_info(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  BtCursor pCur;
    //  int rc;
    //  int i, j;
    //  int up;
    //  int aResult[11];
    //  char zBuf[400];

    //  if( argc!=2 && argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " ID ?UP-CNT?\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pCur = (BtCursor)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if( argc==3 ){
    //    if(TCL.Tcl_GetInt(interp, argv[2], up) ) return TCL.TCL_ERROR;
    //  }else{
    //    up = 0;
    //  }
    //  sqlite3BtreeEnter(pCur.pBtree);
    //  rc = sqlite3BtreeCursorInfo(pCur, aResult, up);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    sqlite3BtreeLeave(pCur.pBtree);
    //    return TCL.TCL_ERROR;
    //  }
    //  j = 0;
    //  for(i=0; i<sizeof(aResult)/sizeof(aResult[0]); i++){
    //    sqlite3_snprintf(40,&zBuf[j]," %d", aResult[i]);
    //    j += strlen(&zBuf[j]);
    //  }
    //  sqlite3BtreeLeave(pCur.pBtree);
    // TCL.Tcl_AppendResult(interp, zBuf[1]);
    //  return SQLITE_OK;
    //}

    /*
    ** Copied from btree.c:
    */
    //static u32 t4Get4byte(unsigned char p){
    //  return (p[0]<<24) | (p[1]<<16) | (p[2]<<8) | p[3];
    //}

    /*
    **   btree_ovfl_info  BTREE  CURSOR
    **
    ** Given a cursor, return the sequence of pages number that form the
    ** overflow pages for the data of the entry that the cursor is point
    ** to.
    */
    //static int btree_ovfl_info(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  Btree pBt;
    //  BtCursor pCur;
    //  Pager pPager;
    //  int rc;
    //  int n;
    //  int dataSize;
    //  u32 pgno;
    //  void pPage;
    //  int aResult[11];
    //  char zElem[100];
    // TCL.Tcl_DString str;

    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(), 
    //                    " BTREE CURSOR", 0);
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  pCur = (BtCursor)sqlite3TestTextToPtr(argv[2]);
    //  if( (*(void**)pCur) != (void*)pBt ){
    //   TCL.Tcl_AppendResult(interp, "Cursor ", argv[2], " does not belong to btree ",
    //       argv[1], 0);
    //    return TCL.TCL_ERROR;
    //  }
    //  sqlite3BtreeEnter(pBt);
    //  pPager = sqlite3BtreePager(pBt);
    //  rc = sqlite3BtreeCursorInfo(pCur, aResult, 0);
    //  if( rc !=0){
    //   TCL.Tcl_AppendResult(interp, errorName(rc), null);;
    //    sqlite3BtreeLeave(pBt);
    //    return TCL.TCL_ERROR;
    //  }
    //  dataSize = pBt.pBt.usableSize;
    // TCL.Tcl_DStringInit(&str);
    //  n = aResult[6] - aResult[8];
    //  n = (n + dataSize - 1)/dataSize;
    //  pgno = (u32)aResult[10];
    //  while( pgno && n-- ){
    //    DbPage pDbPage;
    //    sprintf(zElem, "%d", pgno);
    //   TCL.Tcl_DStringAppendElement(&str, zElem);
    //    if( sqlite3PagerGet(pPager, pgno, pDbPage)!=SQLITE_OK ){
    //     TCL.Tcl_DStringFree(&str);
    //     TCL.Tcl_AppendResult(interp, "unable to get page ", zElem);
    //      sqlite3BtreeLeave(pBt);
    //      return TCL.TCL_ERROR;
    //    }
    //    pPage = sqlite3PagerGetData(pDbPage);
    //    pgno = t4Get4byte((unsigned char*)pPage);
    //    sqlite3PagerUnref(pDbPage);
    //  }
    //  sqlite3BtreeLeave(pBt);
    // TCL.Tcl_DStringResult(interp, str);
    //  return SQLITE_OK;
    //}

    /*
    ** The command is provided for the purpose of setting breakpoints.
    ** in regression test scripts.
    **
    ** By setting a GDB breakpoint on this procedure and executing the
    ** btree_breakpoint command in a test script, we can stop GDB at
    ** the point in the script where the btree_breakpoint command is
    ** inserted.  This is useful for debugging.
    */
    static int btree_breakpoint(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,             /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      return TCL.TCL_OK;
    }

    /*
    ** usage:   varint_test  START  MULTIPLIER  COUNT  INCREMENT
    **
    ** This command tests the putVarint() and getVarint()
    ** routines, both for accuracy and for speed.
    **
    ** An integer is written using putVarint() and read back with
    ** getVarint() and varified to be unchanged.  This repeats COUNT
    ** times.  The first integer is START*MULTIPLIER.  Each iteration
    ** increases the integer by INCREMENT.
    **
    ** This command returns nothing if it works.  It returns an error message
    ** if something goes wrong.
    */
    static int btree_varint_test(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that _invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      int start = 0, mult = 0, count = 0, incr = 0;
      int _in;
      u32 _out = 0;
      int n1, n2, i, j;
      byte[] zBuf = new byte[100];
      if ( argc != 5 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " START MULTIPLIER COUNT incrEMENT\"", 0 );
        return TCL.TCL_ERROR;
      }
      if ( TCL.Tcl_GetInt( interp, argv[1], ref start ) ) return TCL.TCL_ERROR;
      if ( TCL.Tcl_GetInt( interp, argv[2], ref mult ) ) return TCL.TCL_ERROR;
      if ( TCL.Tcl_GetInt( interp, argv[3], ref count ) ) return TCL.TCL_ERROR;
      if ( TCL.Tcl_GetInt( interp, argv[4], ref incr ) ) return TCL.TCL_ERROR;
      _in = start;
      _in *= mult;
      for ( i = 0 ; i < count ; i++ )
      {
        string zErr = "";//char zErr[200];
        n1 = putVarint( zBuf, 0, _in );
        if ( n1 > 9 || n1 < 1 )
        {
          sqlite3_snprintf( 100, ref zErr, "putVarint returned %d - should be between 1 and 9", n1 );
          TCL.Tcl_AppendResult( interp, zErr );
          return TCL.TCL_ERROR;
        }
        n2 = getVarint( zBuf, 0, ref _out );
        if ( n1 != n2 )
        {
          sqlite3_snprintf( 100, ref zErr, "putVarint returned %d and GetVar_int returned %d", n1, n2 );
          TCL.Tcl_AppendResult( interp, zErr );
          return TCL.TCL_ERROR;
        }
        if ( _in != (int)_out )
        {
          sqlite3_snprintf( 100, ref zErr, "Wrote 0x%016llx and got back 0x%016llx", _in, _out );
          TCL.Tcl_AppendResult( interp, zErr );
          return TCL.TCL_ERROR;
        }
        if ( ( _in & 0xffffffff ) == _in )
        {
          u32 _out32 = 0;
          n2 = getVarint32( zBuf, ref _out32 );
          _out = _out32;
          if ( n1 != n2 )
          {
            sqlite3_snprintf( 100, ref zErr, "putVarint returned %d and GetVar_int32 returned %d",
            n1, n2 );
            TCL.Tcl_AppendResult( interp, zErr );
            return TCL.TCL_ERROR;
          }
          if ( _in != (int)_out )
          {
            sqlite3_snprintf( 100, ref zErr, "Wrote 0x%016llx and got back 0x%016llx from GetVar_int32",
            _in, _out );
            TCL.Tcl_AppendResult( interp, zErr );
            return TCL.TCL_ERROR;
          }
        }

        /* _in order to get realistic tim_ings, run getVar_int 19 more times.
        ** This is because getVar_int is called ab_out 20 times more often
        ** than putVarint.
        */
        for ( j = 0 ; j < 19 ; j++ )
        {
          getVarint( zBuf, 0, ref _out );
        }
        _in += incr;
      }
      return TCL.TCL_OK;
    }

    /*
    ** usage:   btree_from_db  DB-HANDLE
    **
    ** This command returns the btree handle for the main database associated
    ** with the database-handle passed as the argument. Example usage:
    **
    ** sqlite3 db test.db
    ** set bt [btree_from_db db]
    */
    static int btree_from_db(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      string zBuf = "";//char zBuf[100];
      WrappedCommand info = null;
      sqlite3 db;
      Btree pBt;
      int iDb = 0;

      if ( argc != 2 && argc != 3 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0].ToString(),
        " DB-HANDLE ?N?\"" );
        return TCL.TCL_ERROR;
      }

      if ( TCL.Tcl_GetCommandInfo( interp, argv[1].ToString(), ref info ) )
      {
        TCL.Tcl_AppendResult( interp, "No such db-handle: \"", argv[1], "\"" );
        return TCL.TCL_ERROR;
      }
      if ( argc == 3 )
      {
        iDb = atoi( argv[2].ToString() );
      }

      db = ( (SqliteDb)info.objClientData ).db;
      Debug.Assert( db != null );

      pBt = db.aDb[iDb].pBt;
      sqlite3_snprintf( 50, ref zBuf, "->%p", pBt );
      if ( TCL.Tcl_CreateCommandPointer( interp, zBuf, pBt ) )
      {
        return TCL.TCL_ERROR;
      }
      else
        TCL.Tcl_SetResult( interp, zBuf, TCL.TCL_VOLATILE );
      return TCL.TCL_OK;
    }


    /*
    ** usage:   btree_set_cache_size ID NCACHE
    **
    ** Set the size of the cache used by btree $ID.
    */
    //static int btree_set_cache_size(
    //  object NotUsed,
    // Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    //  int argc,              /* Number of arguments */
    //  TclObject[] argv      /* Text of each argument */
    //){
    //  int nCache;
    //  Btree pBt;

    //  if( argc!=3 ){
    //   TCL.Tcl_AppendResult(interp, "wrong # args: should be \"", argv[0].ToString(),
    //       " BT NCACHE\"");
    //    return TCL.TCL_ERROR;
    //  }
    //  pBt = (Btree)sqlite3TestTextToPtr(interp,argv[1].ToString());
    //  if(TCL.Tcl_GetInt(interp, argv[2], nCache) ) return TCL.TCL_ERROR;

    //  sqlite3_mutex_enter(pBt.db.mutex);
    //  sqlite3BtreeEnter(pBt);
    //  sqlite3BtreeSetCacheSize(pBt, nCache);
    //  sqlite3BtreeLeave(pBt);
    //  sqlite3_mutex_leave(pBt.db.mutex);

    //  return TCL.TCL_OK;
    //}


    /*
    ** Usage:   btree_ismemdb ID
    **
    ** Return true if the B-Tree is in-memory.
    */
    static int btree_ismemdb(
    object NotUsed,
    Tcl_Interp interp,    /* The TCL interpreter that invoked this command */
    int argc,              /* Number of arguments */
    TclObject[] argv      /* Text of each argument */
    )
    {
      Btree pBt;
      int res;

      if ( argc != 2 )
      {
        TCL.Tcl_AppendResult( interp, "wrong # args: should be \"", argv[0],
        " ID\"" );
        return TCL.TCL_ERROR;
      }
      pBt = (Btree)sqlite3TestTextToPtr( interp, argv[1].ToString() );
      sqlite3_mutex_enter( pBt.db.mutex );
      sqlite3BtreeEnter( pBt );
      res = sqlite3PagerIsMemdb( sqlite3BtreePager( pBt ) ) ? 1 : 0;
      sqlite3BtreeLeave( pBt );
      sqlite3_mutex_leave( pBt.db.mutex );
      TCL.Tcl_SetObjResult( interp, TCL.Tcl_NewBooleanObj( res ) );
      return TCL.TCL_OK;
    }


    /*
    ** Register commands with the TCL interpreter.
    */
    public class _aCmd
    {
      public string zName;
      public Tcl_CmdProc xProc;

      public _aCmd( string zName, Tcl_CmdProc xProc )
      {
        this.zName = zName;
        this.xProc = xProc;
      }
    }


    public static int Sqlitetest3_Init( Tcl_Interp interp )
    {
      _aCmd[] aCmd = new _aCmd[] {
new _aCmd( "btree_open",               (Tcl_CmdProc)btree_open               ),
new _aCmd( "btree_close",              (Tcl_CmdProc)btree_close              ),
new _aCmd( "btree_begin_transaction",  (Tcl_CmdProc)btree_begin_transaction  ),
new _aCmd( "btree_commit",             (Tcl_CmdProc)btree_commit             ),
new _aCmd( "btree_rollback",           (Tcl_CmdProc)btree_rollback           ),
//new _aCmd( "btree_create_table",       (Tcl_CmdProc)btree_create_table       ),
//new _aCmd( "btree_drop_table",         (Tcl_CmdProc)btree_drop_table         ),
//new _aCmd( "btree_clear_table",        (Tcl_CmdProc)btree_clear_table        ),
//new _aCmd( "btree_get_meta",           (Tcl_CmdProc)btree_get_meta           ),
//new _aCmd( "btree_update_meta",        (Tcl_CmdProc)btree_update_meta        ),
new _aCmd( "btree_pager_stats",        (Tcl_CmdProc)btree_pager_stats        ),
new _aCmd( "btree_cursor",             (Tcl_CmdProc)btree_cursor             ),
new _aCmd( "btree_close_cursor",       (Tcl_CmdProc)btree_close_cursor       ),
//new _aCmd( "btree_move_to",            (Tcl_CmdProc)btree_move_to            ),
//new _aCmd( "btree_delete",             (Tcl_CmdProc)btree_delete             ),
new _aCmd( "btree_next",               (Tcl_CmdProc)btree_next               ),
//new _aCmd( "btree_prev",               (Tcl_CmdProc)btree_prev               ),
//new _aCmd( "btree_eof",                (Tcl_CmdProc)btree_eof                ),
//new _aCmd( "btree_keysize",            (Tcl_CmdProc)btree_keysize            ),
//new _aCmd( "btree_key",                (Tcl_CmdProc)btree_key                ),
//new _aCmd( "btree_data",               (Tcl_CmdProc)btree_data               ),
//new _aCmd( "btree_fetch_key",          (Tcl_CmdProc)btree_fetch_key          ),
//new _aCmd( "btree_fetch_data",         (Tcl_CmdProc)btree_fetch_data         ),
new _aCmd( "btree_payload_size",       (Tcl_CmdProc)btree_payload_size       ),
new _aCmd( "btree_first",              (Tcl_CmdProc)btree_first              ),
//new _aCmd( "btree_last",               (Tcl_CmdProc)btree_last               ),
//new _aCmd( "btree_integrity_check",    (Tcl_CmdProc)btree_integrity_check    ),
new _aCmd( "btree_breakpoint",         (Tcl_CmdProc)btree_breakpoint         ),
new _aCmd( "btree_varint_test",        (Tcl_CmdProc)btree_varint_test        ),
//new _aCmd( "btree_begin_statement",    (Tcl_CmdProc)btree_begin_statement    ),
//new _aCmd( "btree_commit_statement",   (Tcl_CmdProc)btree_commit_statement   ),
//new _aCmd( "btree_rollback_statement", (Tcl_CmdProc)btree_rollback_statement ),
new _aCmd( "btree_from_db",            (Tcl_CmdProc)btree_from_db            ),
//new _aCmd( "btree_set_cache_size",     (Tcl_CmdProc)btree_set_cache_size     ),
//new _aCmd( "btree_cursor_info",        (Tcl_CmdProc)btree_cursor_info        ),
//new _aCmd( "btree_ovfl_info",          (Tcl_CmdProc)btree_ovfl_info          ),
//new _aCmd( "btree_cursor_list",        (Tcl_CmdProc)btree_cursor_list        ),
new _aCmd( "btree_ismemdb",        (Tcl_CmdProc)btree_ismemdb       ),
};
      int i;

      for ( i = 0 ; i < aCmd.Length ; i++ )
      { //sizeof(aCmd)/sizeof(aCmd[0]); i++){
        TCL.Tcl_CreateCommand( interp, aCmd[i].zName, aCmd[i].xProc, null, null );
      }

      /* The btree_insert command is implemented using the tcl 'object'
      ** interface, not the string interface like the other commands in this
      ** file. This is so binary data can be inserted into btree tables.
      */
      //Tcl_CreateObjCommand(interp, "btree_insert", btree_insert, 0, 0);
      return TCL.TCL_OK;
    }
  }
#endif
}