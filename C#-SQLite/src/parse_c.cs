#define YYFALLBACK
#define YYWILDCARD

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using u8 = System.Byte;


using YYCODETYPE = System.Int32;
using YYACTIONTYPE = System.Int32;

namespace CS_SQLite3
{
  using sqlite3ParserTOKENTYPE = csSQLite.Token;

  public partial class csSQLite
  {
    /*
    **
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  $Header$
    *************************************************************************
    */

    /* Driver template for the LEMON parser generator.
    ** The author disclaims copyright to this source code.
    */
    /* First off, code is included that follows the "include" declaration
    ** in the input grammar file. */
    //#include <stdio.h>
    //#line 53 "parse.y"

    //#include "sqliteInt.h"
    /*
    ** Disable all error recovery processing in the parser push-down
    ** automaton.
    */
    //#define YYNOERRORRECOVERY 1
    const int YYNOERRORRECOVERY = 1;

    /*
    ** Make yytestcase() the same as testcase()
    */
    //#define yytestcase(X) testcase(X)
    static void yytestcase<T>( T X ) { testcase( X ); }

    /*
    ** An instance of this structure holds information about the
    ** LIMIT clause of a SELECT statement.
    */
    public struct LimitVal
    {
      public Expr pLimit;    /* The LIMIT expression.  NULL if there is no limit */
      public Expr pOffset;   /* The OFFSET expression.  NULL if there is none */
    };

    /*
    ** An instance of this structure is used to store the LIKE,
    ** GLOB, NOT LIKE, and NOT GLOB operators.
    */
    public struct LikeOp
    {
      public Token eOperator;  /* "like" or "glob" or "regexp" */
      public bool not;         /* True if the NOT keyword is present */
    };

    /*
    ** An instance of the following structure describes the event of a
    ** TRIGGER.  "a" is the event type, one of TK_UPDATE, TK_INSERT,
    ** TK_DELETE, or TK_INSTEAD.  If the event is of the form
    **
    **      UPDATE ON (a,b,c)
    **
    ** Then the "b" IdList records the list "a,b,c".
    */
#if !SQLITE_OMIT_TRIGGER
    public struct TrigEvent { public int a; public IdList b; };
#endif
    /*
** An instance of this structure holds the ATTACH key and the key type.
*/
    public struct AttachKey { public int type; public Token key; };

    //#line 725 "parse.y"

    /* This is a utility routine used to set the ExprSpan.zStart and
    ** ExprSpan.zEnd values of pOut so that the span covers the complete
    ** range of text beginning with pStart and going to the end of pEnd.
    */
    static void spanSet( ExprSpan pOut, Token pStart, Token pEnd )
    {
      pOut.zStart = pStart.z;
      pOut.zEnd = pEnd.z.Substring( pEnd.n );
    }

    /* Construct a new Expr object from a single identifier.  Use the
    ** new Expr to populate pOut.  Set the span of pOut to be the identifier
    ** that created the expression.
    */
    static void spanExpr( ExprSpan pOut, Parse pParse, int op, Token pValue )
    {
      pOut.pExpr = sqlite3PExpr( pParse, op, 0, 0, pValue );
      pOut.zStart = pValue.z;
      pOut.zEnd = pValue.z.Substring( pValue.n );
    }
    //#line 820 "parse.y"

    /* This routine constructs a binary expression node out of two ExprSpan
    ** objects and uses the result to populate a new ExprSpan object.
    */
    static void spanBinaryExpr(
    ExprSpan pOut,     /* Write the result here */
    Parse pParse,      /* The parsing context.  Errors accumulate here */
    int op,            /* The binary operation */
    ExprSpan pLeft,    /* The left operand */
    ExprSpan pRight    /* The right operand */
    )
    {
      pOut.pExpr = sqlite3PExpr( pParse, op, pLeft.pExpr, pRight.pExpr, 0 );
      pOut.zStart = pLeft.zStart;
      pOut.zEnd = pRight.zEnd;
    }
    //#line 872 "parse.y"

    /* Construct an expression node for a unary postfix operator
    */
    static void spanUnaryPostfix(
    ExprSpan pOut,        /* Write the new expression node here */
    Parse pParse,         /* Parsing context to record errors */
    int op,               /* The operator */
    ExprSpan pOperand,    /* The operand */
    Token pPostOp         /* The operand token for setting the span */
    )
    {
      pOut.pExpr = sqlite3PExpr( pParse, op, pOperand.pExpr, 0, 0 );
      pOut.zStart = pOperand.zStart;
      pOut.zEnd = pPostOp.z.Substring( pPostOp.n );
    }
    //#line 894 "parse.y"

    /* Construct an expression node for a unary prefix operator
    */
    static void spanUnaryPrefix(
    ExprSpan pOut,        /* Write the new expression node here */
    Parse pParse,         /* Parsing context to record errors */
    int op,               /* The operator */
    ExprSpan pOperand,    /* The operand */
    Token pPreOp          /* The operand token for setting the span */
    )
    {
      pOut.pExpr = sqlite3PExpr( pParse, op, pOperand.pExpr, 0, 0 );
      pOut.zStart = pPreOp.z;
      pOut.zEnd = pOperand.zEnd;
    }
    //#line 123 "parse.c"
    /* Next is all token values, in a form suitable for use by makeheaders.
    ** This section will be null unless lemon is run with the -m switch.
    */
    /*
    ** These constants (all generated automatically by the parser generator)
    ** specify the various kinds of tokens (terminals) that the parser
    ** understands.
    **
    ** Each symbol here is a terminal symbol in the grammar.
    */
    /* Make sure the INTERFACE macro is defined.
    */
#if !INTERFACE
    //# define INTERFACE 1
#endif
    /* The next thing included is series of defines which control
** various aspects of the generated parser.
**    YYCODETYPE         is the data type used for storing terminal
**                       and nonterminal numbers.  "unsigned char" is
**                       used if there are fewer than 250 terminals
**                       and nonterminals.  "int" is used otherwise.
**    YYNOCODE           is a number of type YYCODETYPE which corresponds
**                       to no legal terminal or nonterminal number.  This
**                       number is used to fill in empty slots of the hash
**                       table.
**    YYFALLBACK         If defined, this indicates that one or more tokens
**                       have fall-back values which should be used if the
**                       original value of the token will not parse.
**    YYACTIONTYPE       is the data type used for storing terminal
**                       and nonterminal numbers.  "unsigned char" is
**                       used if there are fewer than 250 rules and
**                       states combined.  "int" is used otherwise.
**    sqlite3ParserTOKENTYPE     is the data type used for minor tokens given
**                       directly to the parser from the tokenizer.
**    YYMINORTYPE        is the data type used for all minor tokens.
**                       This is typically a union of many types, one of
**                       which is sqlite3ParserTOKENTYPE.  The entry in the union
**                       for base tokens is called "yy0".
**    YYSTACKDEPTH       is the maximum depth of the parser's stack.  If
**                       zero the stack is dynamically sized using realloc()
**    sqlite3ParserARG_SDECL     A static variable declaration for the %extra_argument
**    sqlite3ParserARG_PDECL     A parameter declaration for the %extra_argument
**    sqlite3ParserARG_STORE     Code to store %extra_argument into yypParser
**    sqlite3ParserARG_FETCH     Code to extract %extra_argument from yypParser
**    YYNSTATE           the combined number of states.
**    YYNRULE            the number of rules in the grammar
**    YYERRORSYMBOL      is the code number of the error symbol.  If not
**                       defined, then do no error processing.
*/
    //#define YYCODETYPE unsigned short char
    const int YYNOCODE = 252;
    //#define YYACTIONTYPE unsigned short int
    const int YYWILDCARD = 65;
    //#define sqlite3ParserTOKENTYPE Token
    public class YYMINORTYPE
    {
      public int yyinit;
      public sqlite3ParserTOKENTYPE yy0 = new sqlite3ParserTOKENTYPE();
      public Expr yy72;
#if !SQLITE_OMIT_TRIGGER
      public TriggerStep yy145;
#endif
      public ExprList yy148;
      public SrcList yy185;
      public ExprSpan yy190 = new ExprSpan();
      public int yy194;
      public Select yy243;
      public IdList yy254;
#if !SQLITE_OMIT_TRIGGER
      public TrigEvent yy332;
#endif
      public LimitVal yy354;
      public LikeOp yy392;
      public struct _yy497 { public int value; public int mask;}public _yy497 yy497;
    }

#if !YYSTACKDEPTH
    const int YYSTACKDEPTH = 100;
#endif
    //#define sqlite3ParserARG_SDECL Parse pParse;
    //#define sqlite3ParserARG_PDECL ,Parse pParse
    //#define sqlite3ParserARG_FETCH Parse pParse = yypParser.pParse
    //#define sqlite3ParserARG_STORE yypParser.pParse = pParse
    const int YYNSTATE = 619;
    const int YYNRULE = 324;
    //#define YYFALLBACK
    const int YY_NO_ACTION = ( YYNSTATE + YYNRULE + 2 );
    const int YY_ACCEPT_ACTION = ( YYNSTATE + YYNRULE + 1 );
    const int YY_ERROR_ACTION = ( YYNSTATE + YYNRULE );

    /* The yyzerominor constant is used to initialize instances of
    ** YYMINORTYPE objects to zero. */
    YYMINORTYPE yyzerominor = new YYMINORTYPE();//static const YYMINORTYPE yyzerominor = { 0 };

    /* Define the yytestcase() macro to be a no-op if is not already defined
    ** otherwise.
    **
    ** Applications can choose to define yytestcase() in the %include section
    ** to a macro that can assist in verifying code coverage.  For production
    ** code the yytestcase() macro should be turned off.  But it is useful
    ** for testing.
    */
    //#if !yytestcase
    //# define yytestcase(X)
    //#endif

    /* Next are the tables used to determine what action to take based on the
    ** current state and lookahead token.  These tables are used to implement
    ** functions that take a state number and lookahead value and return an
    ** action integer.
    **
    ** Suppose the action integer is N.  Then the action is determined as
    ** follows
    **
    **   0 <= N < YYNSTATE                  Shift N.  That is, push the lookahead
    **                                      token onto the stack and goto state N.
    **
    **   YYNSTATE <= N < YYNSTATE+YYNRULE   Reduce by rule N-YYNSTATE.
    **
    **   N == YYNSTATE+YYNRULE              A syntax error has occurred.
    **
    **   N == YYNSTATE+YYNRULE+1            The parser accepts its input.
    **
    **   N == YYNSTATE+YYNRULE+2            No such action.  Denotes unused
    **                                      slots in the yy_action[] table.
    **
    ** The action table is constructed as a single large table named yy_action[].
    ** Given state S and lookahead X, the action is computed as
    **
    **      yy_action[ yy_shift_ofst[S] + X ]
    **
    ** If the index value yy_shift_ofst[S]+X is out of range or if the value
    ** yy_lookahead[yy_shift_ofst[S]+X] is not equal to X or if yy_shift_ofst[S]
    ** is equal to YY_SHIFT_USE_DFLT, it means that the action is not in the table
    ** and that yy_default[S] should be used instead.
    **
    ** The formula above is for computing the action when the lookahead is
    ** a terminal symbol.  If the lookahead is a non-terminal (as occurs after
    ** a reduce action) then the yy_reduce_ofst[] array is used in place of
    ** the yy_shift_ofst[] array and YY_REDUCE_USE_DFLT is used in place of
    ** YY_SHIFT_USE_DFLT.
    **
    ** The following are the tables generated in this section:
    **
    **  yy_action[]        A single table containing all actions.
    **  yy_lookahead[]     A table containing the lookahead for each entry in
    **                     yy_action.  Used to detect hash collisions.
    **  yy_shift_ofst[]    For each state, the offset into yy_action for
    **                     shifting terminals.
    **  yy_reduce_ofst[]   For each state, the offset into yy_action for
    **                     shifting non-terminals after a reduce.
    **  yy_default[]       Default action for each state.
    */
    static YYACTIONTYPE[] yy_action = new YYACTIONTYPE[]{
/*     0 */   305,  944,  176,  618,    2,  150,  214,  441,   24,   24,
/*    10 */    24,   24,  490,   26,   26,   26,   26,   27,   27,   28,
/*    20 */    28,   28,   29,  216,  415,  416,  212,  415,  416,  448,
/*    30 */   454,   31,   26,   26,   26,   26,   27,   27,   28,   28,
/*    40 */    28,   29,  216,   30,  485,   32,  134,   23,   22,  311,
/*    50 */   458,  459,  455,  455,   25,   25,   24,   24,   24,   24,
/*    60 */   438,   26,   26,   26,   26,   27,   27,   28,   28,   28,
/*    70 */    29,  216,  305,  216,  314,  441,  514,  492,   45,   26,
/*    80 */    26,   26,   26,   27,   27,   28,   28,   28,   29,  216,
/*    90 */   415,  416,  418,  419,  156,  418,  419,  362,  365,  366,
/*   100 */   314,  448,  454,  387,  516,   21,  186,  497,  367,   27,
/*   110 */    27,   28,   28,   28,   29,  216,  415,  416,  417,   23,
/*   120 */    22,  311,  458,  459,  455,  455,   25,   25,   24,   24,
/*   130 */    24,   24,  557,   26,   26,   26,   26,   27,   27,   28,
/*   140 */    28,   28,   29,  216,  305,  228,  506,  135,  470,  218,
/*   150 */   550,  145,  132,  256,  360,  261,  361,  153,  418,  419,
/*   160 */   241,  600,  333,   30,  265,   32,  134,  441,  598,  599,
/*   170 */   230,  228,  492,  448,  454,   57,  508,  330,  132,  256,
/*   180 */   360,  261,  361,  153,  418,  419,  437,   78,  410,  407,
/*   190 */   265,   23,   22,  311,  458,  459,  455,  455,   25,   25,
/*   200 */    24,   24,   24,   24,  344,   26,   26,   26,   26,   27,
/*   210 */    27,   28,   28,   28,   29,  216,  305,  214,  536,  549,
/*   220 */   308,  127,  491,  597,   30,  333,   32,  134,  347,  389,
/*   230 */   431,   63,  333,  357,  417,  441,  509,  333,  417,  537,
/*   240 */   330,  215,  193,  596,  595,  448,  454,  330,   18,  437,
/*   250 */    85,   16,  330,  183,  190,  558,  437,   78,  312,  465,
/*   260 */   466,  437,   85,   23,   22,  311,  458,  459,  455,  455,
/*   270 */    25,   25,   24,   24,   24,   24,  438,   26,   26,   26,
/*   280 */    26,   27,   27,   28,   28,   28,   29,  216,  305,  349,
/*   290 */   221,  316,  597,  191,  380,  333,  474,  234,  347,  383,
/*   300 */   326,  412,  220,  346,  594,  217,  213,  417,  112,  333,
/*   310 */   330,    4,  596,  401,  211,  556,  531,  448,  454,  437,
/*   320 */    79,  217,  555,  517,  330,  336,  515,  461,  461,  471,
/*   330 */   443,  574,  434,  437,   78,   23,   22,  311,  458,  459,
/*   340 */   455,  455,   25,   25,   24,   24,   24,   24,  438,   26,
/*   350 */    26,   26,   26,   27,   27,   28,   28,   28,   29,  216,
/*   360 */   305,  445,  445,  445,  156,  470,  218,  362,  365,  366,
/*   370 */   333,  247,  397,  400,  217,  351,  333,   30,  367,   32,
/*   380 */   134,  390,  282,  281,   39,  330,   41,  432,  547,  448,
/*   390 */   454,  330,  214,  533,  437,   93,  544,  603,    1,  406,
/*   400 */   437,   93,  415,  416,  497,   40,  538,   23,   22,  311,
/*   410 */   458,  459,  455,  455,   25,   25,   24,   24,   24,   24,
/*   420 */   575,   26,   26,   26,   26,   27,   27,   28,   28,   28,
/*   430 */    29,  216,  305,  276,  333,  179,  510,  492,  210,  549,
/*   440 */   322,  415,  416,  222,  192,  387,  323,  240,  417,  330,
/*   450 */   559,   63,  415,  416,  417,  619,  410,  407,  437,   71,
/*   460 */   417,  448,  454,  539,  574,   28,   28,   28,   29,  216,
/*   470 */   418,  419,  438,  338,  465,  466,  403,   43,  438,   23,
/*   480 */    22,  311,  458,  459,  455,  455,   25,   25,   24,   24,
/*   490 */    24,   24,  497,   26,   26,   26,   26,   27,   27,   28,
/*   500 */    28,   28,   29,  216,  305,  429,  209,  135,  513,  418,
/*   510 */   419,  433,  233,   64,  390,  282,  281,  441,   66,  544,
/*   520 */   418,  419,  415,  416,  156,  214,  405,  362,  365,  366,
/*   530 */   549,  252,  492,  448,  454,  493,  217,    8,  367,  497,
/*   540 */   438,  608,   63,  208,  299,  417,  494,  472,  548,  200,
/*   550 */   196,   23,   22,  311,  458,  459,  455,  455,   25,   25,
/*   560 */    24,   24,   24,   24,  388,   26,   26,   26,   26,   27,
/*   570 */    27,   28,   28,   28,   29,  216,  305,  479,  254,  356,
/*   580 */   530,   60,  519,  520,  438,  441,  391,  333,  358,    7,
/*   590 */   418,  419,  333,  480,  330,  374,  197,  137,  462,  501,
/*   600 */   449,  450,  330,  437,    9,  448,  454,  330,  481,  487,
/*   610 */   521,  437,   72,  569,  417,  436,  437,   67,  488,  435,
/*   620 */   522,  452,  453,   23,   22,  311,  458,  459,  455,  455,
/*   630 */    25,   25,   24,   24,   24,   24,  333,   26,   26,   26,
/*   640 */    26,   27,   27,   28,   28,   28,   29,  216,  305,  333,
/*   650 */   451,  330,  268,  392,  463,  333,   65,  333,  370,  436,
/*   660 */   437,   76,  313,  435,  330,  150,  185,  441,  475,  333,
/*   670 */   330,  501,  330,  437,   97,   29,  216,  448,  454,  437,
/*   680 */    96,  437,  101,  355,  330,  242,  417,  336,  154,  461,
/*   690 */   461,  354,  571,  437,   99,   23,   22,  311,  458,  459,
/*   700 */   455,  455,   25,   25,   24,   24,   24,   24,  333,   26,
/*   710 */    26,   26,   26,   27,   27,   28,   28,   28,   29,  216,
/*   720 */   305,  333,  248,  330,  264,   56,  336,  333,  461,  461,
/*   730 */   864,  335,  437,  104,  378,  441,  330,  417,  333,  417,
/*   740 */   567,  333,  330,  307,  566,  437,  105,  442,  265,  448,
/*   750 */   454,  437,  126,  330,  572,  520,  330,  336,  379,  461,
/*   760 */   461,  317,  437,  128,  194,  437,   59,   23,   22,  311,
/*   770 */   458,  459,  455,  455,   25,   25,   24,   24,   24,   24,
/*   780 */   333,   26,   26,   26,   26,   27,   27,   28,   28,   28,
/*   790 */    29,  216,  305,  333,  136,  330,  467,  479,  438,  333,
/*   800 */   352,  333,  611,  303,  437,  102,  201,  137,  330,  417,
/*   810 */   456,  178,  333,  480,  330,  417,  330,  437,   77,  486,
/*   820 */   249,  448,  454,  437,  100,  437,   68,  330,  481,  469,
/*   830 */   343,  616,  934,  341,  934,  417,  437,   98,  489,   23,
/*   840 */    22,  311,  458,  459,  455,  455,   25,   25,   24,   24,
/*   850 */    24,   24,  333,   26,   26,   26,   26,   27,   27,   28,
/*   860 */    28,   28,   29,  216,  305,  333,  399,  330,  164,  264,
/*   870 */   205,  333,  264,  334,  612,  250,  437,  129,  409,    2,
/*   880 */   330,  325,  175,  333,  417,  214,  330,  417,  417,  437,
/*   890 */   130,  468,  468,  448,  454,  437,  131,  398,  330,  257,
/*   900 */   336,  259,  461,  461,  438,  154,  229,  437,   69,  318,
/*   910 */   258,   23,   33,  311,  458,  459,  455,  455,   25,   25,
/*   920 */    24,   24,   24,   24,  333,   26,   26,   26,   26,   27,
/*   930 */    27,   28,   28,   28,   29,  216,  305,  333,  155,  330,
/*   940 */   531,  264,  414,  333,  264,  472,  339,  200,  437,   80,
/*   950 */   542,  499,  330,  151,  541,  333,  417,  417,  330,  417,
/*   960 */   307,  437,   81,  535,  534,  448,  454,  437,   70,   47,
/*   970 */   330,  616,  933,  543,  933,  420,  421,  422,  319,  437,
/*   980 */    82,  320,  304,  613,   22,  311,  458,  459,  455,  455,
/*   990 */    25,   25,   24,   24,   24,   24,  333,   26,   26,   26,
/*  1000 */    26,   27,   27,   28,   28,   28,   29,  216,  305,  333,
/*  1010 */   209,  330,  364,  206,  612,  333,  528,  565,  377,  565,
/*  1020 */   437,   83,  525,  526,  330,  615,  545,  333,  501,  577,
/*  1030 */   330,  333,  290,  437,   84,  426,  396,  448,  454,  437,
/*  1040 */    86,  590,  330,  417,  438,  141,  330,  438,  413,  423,
/*  1050 */   417,  437,   87,  424,  327,  437,   88,  311,  458,  459,
/*  1060 */   455,  455,   25,   25,   24,   24,   24,   24,  388,   26,
/*  1070 */    26,   26,   26,   27,   27,   28,   28,   28,   29,  216,
/*  1080 */    35,  340,  286,    3,  333,  270,  333,  329,  416,  142,
/*  1090 */   384,  321,  276,  425,  144,   35,  340,  337,    3,  330,
/*  1100 */     6,  330,  329,  416,  304,  614,  276,  417,  437,   73,
/*  1110 */   437,   74,  337,  333,  328,  342,  427,  333,  439,  333,
/*  1120 */   540,  417,  155,   47,  289,  474,  287,  274,  330,  272,
/*  1130 */   342,  417,  330,  350,  330,  277,  276,  437,   89,  243,
/*  1140 */   474,  437,   90,  437,   91,   38,   37,  615,  333,  584,
/*  1150 */   244,  417,  428,  276,   36,  331,  332,   46,  245,  443,
/*  1160 */    38,   37,  507,  330,  202,  203,  204,  417,  417,   36,
/*  1170 */   331,  332,  437,   92,  443,  198,  267,  214,  155,  586,
/*  1180 */   235,  236,  237,  143,  239,  348,  133,  583,  440,  246,
/*  1190 */   445,  445,  445,  446,  447,   10,  587,  276,   20,   42,
/*  1200 */   172,  417,  294,  333,  288,  445,  445,  445,  446,  447,
/*  1210 */    10,  295,  417,   35,  340,  219,    3,  149,  330,  484,
/*  1220 */   329,  416,  333,  170,  276,  574,   48,  437,   75,  169,
/*  1230 */   337,   19,  171,  251,  444,  415,  416,  330,  333,  417,
/*  1240 */   588,  345,  276,  177,  353,  498,  437,   17,  342,  417,
/*  1250 */   483,  253,  255,  330,  276,  496,  417,  417,  474,  333,
/*  1260 */   504,  505,  437,   94,  369,  417,  155,  231,  359,  417,
/*  1270 */   417,  518,  523,  474,  330,  395,  291,  281,   38,   37,
/*  1280 */   500,  306,  315,  437,   95,  232,  214,   36,  331,  332,
/*  1290 */   524,  502,  443,  188,  189,  417,  262,  292,  532,  263,
/*  1300 */   551,  260,  269,  515,  271,  273,  417,  443,  570,  402,
/*  1310 */   155,  417,  527,  417,  417,  417,  275,  417,  280,  417,
/*  1320 */   417,  382,  385,  445,  445,  445,  446,  447,   10,  528,
/*  1330 */   386,  417,  283,  417,  284,  285,  417,  417,  445,  445,
/*  1340 */   445,  582,  593,  293,  107,  417,  296,  417,  297,  417,
/*  1350 */   417,  607,  578,  529,  151,  300,  417,  417,  417,  226,
/*  1360 */   579,  417,   54,  417,  158,  591,  417,   54,  225,  610,
/*  1370 */   227,  302,  546,  552,  301,  553,  554,  371,  560,  159,
/*  1380 */   375,  373,  207,  160,   51,  562,  563,  161,  117,  278,
/*  1390 */   381,  140,  573,  163,  181,  393,  394,  118,  119,  120,
/*  1400 */   180,  580,  121,  123,  324,  605,  604,  606,   55,  609,
/*  1410 */   589,  309,  224,   62,   58,  103,  411,  111,  238,  430,
/*  1420 */   199,  174,  660,  661,  662,  146,  147,  460,  310,  457,
/*  1430 */    34,  476,  464,  473,  182,  195,  148,  477,    5,  478,
/*  1440 */   482,   12,  138,   44,   11,  106,  495,  511,  512,  503,
/*  1450 */   223,   49,  363,  108,  109,  152,  266,   50,  110,  157,
/*  1460 */   258,  372,  184,  561,  139,  113,  151,  162,  279,  115,
/*  1470 */   376,   15,  576,  116,  165,   52,   13,  368,  581,   53,
/*  1480 */   167,  166,  585,  122,  124,  114,  592,  564,  568,  168,
/*  1490 */    14,   61,  601,  602,  173,  298,  125,  408,  187,  617,
/*  1500 */   945,  945,  404,
};
    static YYCODETYPE[] yy_lookahead = new YYCODETYPE[]{
/*     0 */    19,  142,  143,  144,  145,   24,  116,   26,   75,   76,
/*    10 */    77,   78,   25,   80,   81,   82,   83,   84,   85,   86,
/*    20 */    87,   88,   89,   90,   26,   27,  160,   26,   27,   48,
/*    30 */    49,   79,   80,   81,   82,   83,   84,   85,   86,   87,
/*    40 */    88,   89,   90,  222,  223,  224,  225,   66,   67,   68,
/*    50 */    69,   70,   71,   72,   73,   74,   75,   76,   77,   78,
/*    60 */   194,   80,   81,   82,   83,   84,   85,   86,   87,   88,
/*    70 */    89,   90,   19,   90,   19,   94,  174,   25,   25,   80,
/*    80 */    81,   82,   83,   84,   85,   86,   87,   88,   89,   90,
/*    90 */    26,   27,   94,   95,   96,   94,   95,   99,  100,  101,
/*   100 */    19,   48,   49,  150,  174,   52,  119,  166,  110,   84,
/*   110 */    85,   86,   87,   88,   89,   90,   26,   27,  165,   66,
/*   120 */    67,   68,   69,   70,   71,   72,   73,   74,   75,   76,
/*   130 */    77,   78,  186,   80,   81,   82,   83,   84,   85,   86,
/*   140 */    87,   88,   89,   90,   19,   90,  205,   95,   84,   85,
/*   150 */   186,   96,   97,   98,   99,  100,  101,  102,   94,   95,
/*   160 */   195,   97,  150,  222,  109,  224,  225,   26,  104,  105,
/*   170 */   217,   90,  120,   48,   49,   50,   86,  165,   97,   98,
/*   180 */    99,  100,  101,  102,   94,   95,  174,  175,    1,    2,
/*   190 */   109,   66,   67,   68,   69,   70,   71,   72,   73,   74,
/*   200 */    75,   76,   77,   78,  191,   80,   81,   82,   83,   84,
/*   210 */    85,   86,   87,   88,   89,   90,   19,  116,   35,  150,
/*   220 */   155,   24,  208,  150,  222,  150,  224,  225,  216,  128,
/*   230 */   161,  162,  150,  221,  165,   94,   23,  150,  165,   56,
/*   240 */   165,  197,  160,  170,  171,   48,   49,  165,  204,  174,
/*   250 */   175,   22,  165,   24,  185,  186,  174,  175,  169,  170,
/*   260 */   171,  174,  175,   66,   67,   68,   69,   70,   71,   72,
/*   270 */    73,   74,   75,   76,   77,   78,  194,   80,   81,   82,
/*   280 */    83,   84,   85,   86,   87,   88,   89,   90,   19,  214,
/*   290 */   215,  108,  150,   25,  229,  150,   64,  148,  216,  234,
/*   300 */   146,  147,  215,  221,  231,  232,  152,  165,  154,  150,
/*   310 */   165,  196,  170,  171,  160,  181,  182,   48,   49,  174,
/*   320 */   175,  232,  188,  165,  165,  112,   94,  114,  115,  166,
/*   330 */    98,   55,  174,  174,  175,   66,   67,   68,   69,   70,
/*   340 */    71,   72,   73,   74,   75,   76,   77,   78,  194,   80,
/*   350 */    81,   82,   83,   84,   85,   86,   87,   88,   89,   90,
/*   360 */    19,  129,  130,  131,   96,   84,   85,   99,  100,  101,
/*   370 */   150,  226,  218,  231,  232,  216,  150,  222,  110,  224,
/*   380 */   225,  105,  106,  107,  135,  165,  137,  172,  173,   48,
/*   390 */    49,  165,  116,  183,  174,  175,  181,  242,   22,  245,
/*   400 */   174,  175,   26,   27,  166,  136,  183,   66,   67,   68,
/*   410 */    69,   70,   71,   72,   73,   74,   75,   76,   77,   78,
/*   420 */    11,   80,   81,   82,   83,   84,   85,   86,   87,   88,
/*   430 */    89,   90,   19,  150,  150,   23,   23,   25,  160,  150,
/*   440 */   220,   26,   27,  205,  160,  150,  220,  158,  165,  165,
/*   450 */   161,  162,   26,   27,  165,    0,    1,    2,  174,  175,
/*   460 */   165,   48,   49,  183,   55,   86,   87,   88,   89,   90,
/*   470 */    94,   95,  194,  169,  170,  171,  193,  136,  194,   66,
/*   480 */    67,   68,   69,   70,   71,   72,   73,   74,   75,   76,
/*   490 */    77,   78,  166,   80,   81,   82,   83,   84,   85,   86,
/*   500 */    87,   88,   89,   90,   19,  153,  160,   95,   23,   94,
/*   510 */    95,  173,  217,   22,  105,  106,  107,   26,   22,  181,
/*   520 */    94,   95,   26,   27,   96,  116,  243,   99,  100,  101,
/*   530 */   150,  205,  120,   48,   49,  120,  232,   22,  110,  166,
/*   540 */   194,  161,  162,  236,  163,  165,  120,  166,  167,  168,
/*   550 */   160,   66,   67,   68,   69,   70,   71,   72,   73,   74,
/*   560 */    75,   76,   77,   78,  218,   80,   81,   82,   83,   84,
/*   570 */    85,   86,   87,   88,   89,   90,   19,   12,  205,  150,
/*   580 */    23,  235,  190,  191,  194,   94,  240,  150,   86,   74,
/*   590 */    94,   95,  150,   28,  165,  237,  206,  207,   23,  150,
/*   600 */    48,   49,  165,  174,  175,   48,   49,  165,   43,   31,
/*   610 */    45,  174,  175,   21,  165,  113,  174,  175,   40,  117,
/*   620 */    55,   69,   70,   66,   67,   68,   69,   70,   71,   72,
/*   630 */    73,   74,   75,   76,   77,   78,  150,   80,   81,   82,
/*   640 */    83,   84,   85,   86,   87,   88,   89,   90,   19,  150,
/*   650 */    98,  165,   23,   61,   23,  150,   25,  150,   19,  113,
/*   660 */   174,  175,  213,  117,  165,   24,  196,   26,   23,  150,
/*   670 */   165,  150,  165,  174,  175,   89,   90,   48,   49,  174,
/*   680 */   175,  174,  175,   19,  165,  198,  165,  112,   49,  114,
/*   690 */   115,   27,  100,  174,  175,   66,   67,   68,   69,   70,
/*   700 */    71,   72,   73,   74,   75,   76,   77,   78,  150,   80,
/*   710 */    81,   82,   83,   84,   85,   86,   87,   88,   89,   90,
/*   720 */    19,  150,  150,  165,  150,   24,  112,  150,  114,  115,
/*   730 */   138,   19,  174,  175,  213,   94,  165,  165,  150,  165,
/*   740 */    29,  150,  165,  104,   33,  174,  175,  166,  109,   48,
/*   750 */    49,  174,  175,  165,  190,  191,  165,  112,   47,  114,
/*   760 */   115,  187,  174,  175,  160,  174,  175,   66,   67,   68,
/*   770 */    69,   70,   71,   72,   73,   74,   75,   76,   77,   78,
/*   780 */   150,   80,   81,   82,   83,   84,   85,   86,   87,   88,
/*   790 */    89,   90,   19,  150,  150,  165,  233,   12,  194,  150,
/*   800 */   150,  150,  248,  249,  174,  175,  206,  207,  165,  165,
/*   810 */    98,   23,  150,   28,  165,  165,  165,  174,  175,  177,
/*   820 */   150,   48,   49,  174,  175,  174,  175,  165,   43,  233,
/*   830 */    45,   22,   23,  228,   25,  165,  174,  175,  177,   66,
/*   840 */    67,   68,   69,   70,   71,   72,   73,   74,   75,   76,
/*   850 */    77,   78,  150,   80,   81,   82,   83,   84,   85,   86,
/*   860 */    87,   88,   89,   90,   19,  150,   97,  165,   25,  150,
/*   870 */   160,  150,  150,  150,   65,  209,  174,  175,  144,  145,
/*   880 */   165,  246,  247,  150,  165,  116,  165,  165,  165,  174,
/*   890 */   175,  129,  130,   48,   49,  174,  175,  128,  165,   98,
/*   900 */   112,  177,  114,  115,  194,   49,  187,  174,  175,  187,
/*   910 */   109,   66,   67,   68,   69,   70,   71,   72,   73,   74,
/*   920 */    75,   76,   77,   78,  150,   80,   81,   82,   83,   84,
/*   930 */    85,   86,   87,   88,   89,   90,   19,  150,   25,  165,
/*   940 */   182,  150,  150,  150,  150,  166,  167,  168,  174,  175,
/*   950 */   166,   23,  165,   25,  177,  150,  165,  165,  165,  165,
/*   960 */   104,  174,  175,   97,   98,   48,   49,  174,  175,  126,
/*   970 */   165,   22,   23,  177,   25,    7,    8,    9,  187,  174,
/*   980 */   175,  187,   22,   23,   67,   68,   69,   70,   71,   72,
/*   990 */    73,   74,   75,   76,   77,   78,  150,   80,   81,   82,
/*  1000 */    83,   84,   85,   86,   87,   88,   89,   90,   19,  150,
/*  1010 */   160,  165,  178,  160,   65,  150,  103,  105,  106,  107,
/*  1020 */   174,  175,    7,    8,  165,   65,  166,  150,  150,  199,
/*  1030 */   165,  150,  209,  174,  175,  150,  209,   48,   49,  174,
/*  1040 */   175,  199,  165,  165,  194,    6,  165,  194,  149,  149,
/*  1050 */   165,  174,  175,  149,  149,  174,  175,   68,   69,   70,
/*  1060 */    71,   72,   73,   74,   75,   76,   77,   78,  218,   80,
/*  1070 */    81,   82,   83,   84,   85,   86,   87,   88,   89,   90,
/*  1080 */    19,   20,   16,   22,  150,   16,  150,   26,   27,  151,
/*  1090 */   240,  213,  150,   13,  151,   19,   20,   36,   22,  165,
/*  1100 */    25,  165,   26,   27,   22,   23,  150,  165,  174,  175,
/*  1110 */   174,  175,   36,  150,  159,   54,  150,  150,  194,  150,
/*  1120 */    23,  165,   25,  126,   58,   64,   60,   58,  165,   60,
/*  1130 */    54,  165,  165,  123,  165,  193,  150,  174,  175,  199,
/*  1140 */    64,  174,  175,  174,  175,   84,   85,   65,  150,  193,
/*  1150 */   200,  165,  150,  150,   93,   94,   95,  124,  201,   98,
/*  1160 */    84,   85,   86,  165,  105,  106,  107,  165,  165,   93,
/*  1170 */    94,   95,  174,  175,   98,    5,   23,  116,   25,  193,
/*  1180 */    10,   11,   12,   13,   14,  122,  150,   17,  203,  202,
/*  1190 */   129,  130,  131,  132,  133,  134,  193,  150,  125,  135,
/*  1200 */    30,  165,   32,  150,  138,  129,  130,  131,  132,  133,
/*  1210 */   134,   41,  165,   19,   20,  227,   22,  118,  165,  157,
/*  1220 */    26,   27,  150,   53,  150,   55,  104,  174,  175,   59,
/*  1230 */    36,   22,   62,  210,  150,   26,   27,  165,  150,  165,
/*  1240 */   193,  150,  150,  157,  121,  211,  174,  175,   54,  165,
/*  1250 */   150,  210,  210,  165,  150,  150,  165,  165,   64,  150,
/*  1260 */   211,  211,  174,  175,   23,  165,   25,  193,  104,  165,
/*  1270 */   165,  176,  176,   64,  165,  105,  106,  107,   84,   85,
/*  1280 */   150,  111,   46,  174,  175,  193,  116,   93,   94,   95,
/*  1290 */   184,  150,   98,   84,   85,  165,  150,  193,  150,  150,
/*  1300 */   150,  176,  150,   94,  150,  150,  165,   98,   23,  139,
/*  1310 */    25,  165,  178,  165,  165,  165,  150,  165,  150,  165,
/*  1320 */   165,  150,  150,  129,  130,  131,  132,  133,  134,  103,
/*  1330 */   150,  165,  150,  165,  150,  150,  165,  165,  129,  130,
/*  1340 */   131,  150,  150,  150,   22,  165,  150,  165,  150,  165,
/*  1350 */   165,  150,   23,  176,   25,  179,  165,  165,  165,   90,
/*  1360 */    23,  165,   25,  165,  156,   23,  165,   25,  230,   23,
/*  1370 */   230,   25,  184,  176,  179,  176,  176,   18,  157,  156,
/*  1380 */    44,  157,  157,  156,  135,  157,  239,  156,   22,  238,
/*  1390 */   157,   66,  189,  189,  219,  157,   18,  192,  192,  192,
/*  1400 */   219,  199,  192,  189,  157,  157,   39,  157,  241,   37,
/*  1410 */   199,  250,  180,  244,  241,  164,    1,  180,   15,   23,
/*  1420 */    22,  247,  118,  118,  118,  118,  118,  113,  250,   98,
/*  1430 */    22,   11,   23,   23,   22,   22,   25,   23,   34,   23,
/*  1440 */    23,   34,  118,   25,   25,   22,  120,   23,   23,   27,
/*  1450 */    50,   22,   50,   22,   22,   34,   23,   22,   22,  102,
/*  1460 */   109,   19,   24,   20,   38,  104,   25,  104,  138,   22,
/*  1470 */    42,    5,    1,  108,  127,   74,   22,   50,    1,   74,
/*  1480 */    16,  119,   20,  119,  108,   51,  128,   57,   51,  121,
/*  1490 */    22,   16,   23,   23,   15,  140,  127,    3,   22,    4,
/*  1500 */   251,  251,   63,
};
    const int YY_SHIFT_USE_DFLT = ( -111 );
    const int YY_SHIFT_MAX = 408;
    static short[] yy_shift_ofst = new short[]{
/*     0 */   187, 1061, 1170, 1061, 1194, 1194,   -2,   64,   64,  -19,
/*    10 */  1194, 1194, 1194, 1194, 1194,  276,    1,  125, 1076, 1194,
/*    20 */  1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194,
/*    30 */  1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194,
/*    40 */  1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194,
/*    50 */  1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194, 1194,  -48,
/*    60 */   409,    1,    1,  141,  281,  281, -110,   53,  197,  269,
/*    70 */   341,  413,  485,  557,  629,  701,  773,  845,  773,  773,
/*    80 */   773,  773,  773,  773,  773,  773,  773,  773,  773,  773,
/*    90 */   773,  773,  773,  773,  773,  773,  917,  989,  989,  -67,
/*   100 */   -67,   -1,   -1,   55,   25,  379,    1,    1,    1,    1,
/*   110 */     1,  639,  592,    1,    1,    1,    1,    1,    1,    1,
/*   120 */     1,    1,    1,    1,    1,    1,  586,  141,  -17, -111,
/*   130 */  -111, -111, 1209,   81,  376,  415,  426,  496,   90,  565,
/*   140 */   565,    1,    1,    1,    1,    1,    1,    1,    1,    1,
/*   150 */     1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
/*   160 */     1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
/*   170 */     1,    1,    1,    1,  809,  949,  455,  641,  641,  641,
/*   180 */   769,  101, -110, -110, -110, -111, -111, -111,  232,  232,
/*   190 */   268,  428,  213,  575,  645,  785,  788,  412,  968,  502,
/*   200 */   491,   52,  183,  183,  183,  614,  614,  711,  912,  614,
/*   210 */   614,  614,  614,  229,  546,  -13,  141,  762,  762,  249,
/*   220 */   578,  578,  664,  578,  856,  578,  141,  578,  141,  913,
/*   230 */   843,  664,  664,  843, 1039, 1039, 1039, 1039, 1080, 1080,
/*   240 */  1075, -110,  997, 1010, 1033, 1063, 1073, 1064, 1099, 1099,
/*   250 */  1122, 1123, 1122, 1123, 1122, 1123, 1164, 1164, 1236, 1164,
/*   260 */  1226, 1164, 1322, 1269, 1269, 1236, 1164, 1164, 1164, 1322,
/*   270 */  1359, 1099, 1359, 1099, 1359, 1099, 1099, 1336, 1249, 1359,
/*   280 */  1099, 1325, 1325, 1366,  997, 1099, 1378, 1378, 1378, 1378,
/*   290 */   997, 1325, 1366, 1099, 1367, 1367, 1099, 1099, 1372, -111,
/*   300 */  -111, -111, -111, -111, -111,  552, 1066, 1059, 1069,  960,
/*   310 */  1082,  712,  631,  928,  801, 1015,  866, 1097, 1153, 1241,
/*   320 */  1285, 1329, 1337, 1342,  515, 1346, 1415, 1403, 1396, 1398,
/*   330 */  1304, 1305, 1306, 1307, 1308, 1331, 1314, 1408, 1409, 1410,
/*   340 */  1412, 1420, 1413, 1414, 1411, 1416, 1417, 1418, 1404, 1419,
/*   350 */  1407, 1418, 1326, 1423, 1421, 1422, 1324, 1424, 1425, 1426,
/*   360 */  1400, 1429, 1402, 1431, 1433, 1432, 1435, 1427, 1436, 1357,
/*   370 */  1351, 1442, 1443, 1438, 1361, 1428, 1430, 1434, 1441, 1437,
/*   380 */  1330, 1363, 1447, 1466, 1471, 1365, 1401, 1405, 1347, 1454,
/*   390 */  1362, 1477, 1464, 1368, 1462, 1364, 1376, 1369, 1468, 1358,
/*   400 */  1469, 1470, 1475, 1439, 1479, 1355, 1476, 1494, 1495,
};
    const int YY_REDUCE_USE_DFLT = ( -180 );
    const int YY_REDUCE_MAX = 304;
    static short[] yy_reduce_ofst = new short[]{
/*     0 */  -141,   82,  154,  284,   12,   75,   69,   73,  142,  -59,
/*    10 */   145,   87,  159,  220,  226,  346,  289,  155,  429,  437,
/*    20 */   442,  486,  499,  505,  507,  519,  558,  571,  577,  588,
/*    30 */   591,  630,  643,  649,  651,  662,  702,  715,  721,  733,
/*    40 */   774,  787,  793,  805,  846,  859,  865,  877,  881,  934,
/*    50 */   936,  963,  967,  969,  998, 1053, 1072, 1088, 1109, -179,
/*    60 */   850,  283,  380,  381,   89,  304,  390,    2,    2,    2,
/*    70 */     2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
/*    80 */     2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
/*    90 */     2,    2,    2,    2,    2,    2,    2,    2,    2,    2,
/*   100 */     2,    2,    2,  215,    2,    2,  449,  574,  719,  722,
/*   110 */   791,  134,   65,  942,  521,  794,  -47,  878,  956,  986,
/*   120 */  1003, 1047, 1074, 1092,  295, 1104,    2,  779,    2,    2,
/*   130 */     2,    2,  158,  338,  572,  644,  650,  670,  723,  392,
/*   140 */   564,  792,  885,  966, 1002, 1036,  723, 1084, 1091, 1100,
/*   150 */  1105, 1130, 1141, 1146, 1148, 1149, 1150, 1152, 1154, 1155,
/*   160 */  1166, 1168, 1171, 1172, 1180, 1182, 1184, 1185, 1191, 1192,
/*   170 */  1193, 1196, 1198, 1201,  554,  554,  734,  238,  326,  373,
/*   180 */  -134,  278,  604,  710,  853,   44,  600,  635,  -98,  -70,
/*   190 */   -54,  -36,  -35,  -35,  -35,   13,  -35,   14,  149,  115,
/*   200 */   163,   14,  210,  223,  280,  -35,  -35,  307,  358,  -35,
/*   210 */   -35,  -35,  -35,  352,  470,  487,  581,  563,  596,  605,
/*   220 */   642,  661,  666,  724,  758,  777,  784,  796,  860,  834,
/*   230 */   830,  823,  827,  842,  899,  900,  904,  905,  938,  943,
/*   240 */   955,  924,  940,  950,  957,  987,  985,  988, 1062, 1086,
/*   250 */  1023, 1034, 1041, 1049, 1042, 1050, 1095, 1096, 1106, 1125,
/*   260 */  1134, 1177, 1176, 1138, 1140, 1188, 1197, 1199, 1200, 1195,
/*   270 */  1208, 1221, 1223, 1224, 1227, 1225, 1228, 1151, 1147, 1231,
/*   280 */  1233, 1203, 1204, 1175, 1202, 1238, 1205, 1206, 1207, 1210,
/*   290 */  1211, 1214, 1181, 1247, 1167, 1173, 1248, 1250, 1169, 1251,
/*   300 */  1232, 1237, 1174, 1161, 1178,
};
    static YYACTIONTYPE[] yy_default = new YYACTIONTYPE[] {
/*     0 */   624,  859,  943,  943,  859,  943,  943,  888,  888,  747,
/*    10 */   857,  943,  943,  943,  943,  943,  943,  917,  943,  943,
/*    20 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*    30 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*    40 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*    50 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  831,
/*    60 */   943,  943,  943,  663,  888,  888,  751,  782,  943,  943,
/*    70 */   943,  943,  943,  943,  943,  943,  783,  943,  861,  856,
/*    80 */   852,  854,  853,  860,  784,  773,  780,  787,  762,  901,
/*    90 */   789,  790,  796,  797,  918,  916,  819,  818,  837,  821,
/*   100 */   843,  820,  830,  655,  822,  823,  943,  943,  943,  943,
/*   110 */   943,  716,  650,  943,  943,  943,  943,  943,  943,  943,
/*   120 */   943,  943,  943,  943,  943,  943,  824,  943,  825,  838,
/*   130 */   839,  840,  943,  943,  943,  943,  943,  943,  943,  943,
/*   140 */   943,  630,  943,  943,  943,  943,  943,  943,  943,  943,
/*   150 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*   160 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  872,
/*   170 */   943,  921,  923,  943,  943,  943,  624,  747,  747,  747,
/*   180 */   943,  943,  943,  943,  943,  741,  751,  935,  943,  943,
/*   190 */   707,  943,  943,  943,  943,  943,  943,  943,  632,  739,
/*   200 */   665,  749,  943,  943,  943,  652,  728,  894,  943,  908,
/*   210 */   906,  730,  792,  943,  739,  748,  943,  943,  943,  855,
/*   220 */   776,  776,  764,  776,  686,  776,  943,  776,  943,  689,
/*   230 */   786,  764,  764,  786,  629,  629,  629,  629,  640,  640,
/*   240 */   706,  943,  786,  777,  779,  769,  781,  943,  755,  755,
/*   250 */   763,  768,  763,  768,  763,  768,  718,  718,  703,  718,
/*   260 */   689,  718,  865,  869,  869,  703,  718,  718,  718,  865,
/*   270 */   647,  755,  647,  755,  647,  755,  755,  898,  900,  647,
/*   280 */   755,  720,  720,  798,  786,  755,  727,  727,  727,  727,
/*   290 */   786,  720,  798,  755,  920,  920,  755,  755,  928,  673,
/*   300 */   691,  691,  935,  940,  940,  943,  943,  943,  943,  943,
/*   310 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*   320 */   943,  943,  943,  943,  874,  943,  943,  638,  943,  657,
/*   330 */   805,  810,  806,  943,  807,  943,  733,  943,  943,  943,
/*   340 */   943,  943,  943,  943,  943,  943,  943,  858,  943,  770,
/*   350 */   943,  778,  943,  943,  943,  943,  943,  943,  943,  943,
/*   360 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*   370 */   943,  943,  943,  943,  943,  943,  943,  896,  897,  943,
/*   380 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*   390 */   943,  943,  943,  943,  943,  943,  943,  943,  943,  943,
/*   400 */   943,  943,  943,  927,  943,  943,  930,  625,  943,  620,
/*   410 */   622,  623,  627,  628,  631,  657,  658,  660,  661,  662,
/*   420 */   633,  634,  635,  636,  637,  639,  643,  641,  642,  644,
/*   430 */   651,  653,  672,  674,  676,  737,  738,  802,  731,  732,
/*   440 */   736,  659,  813,  804,  808,  809,  811,  812,  826,  827,
/*   450 */   829,  835,  842,  845,  828,  833,  834,  836,  841,  844,
/*   460 */   734,  735,  848,  666,  667,  670,  671,  884,  886,  885,
/*   470 */   887,  669,  668,  814,  817,  850,  851,  909,  910,  911,
/*   480 */   912,  913,  846,  756,  849,  832,  771,  774,  775,  772,
/*   490 */   740,  750,  758,  759,  760,  761,  745,  746,  752,  767,
/*   500 */   800,  801,  765,  766,  753,  754,  742,  743,  744,  847,
/*   510 */   803,  815,  816,  677,  678,  810,  679,  680,  681,  719,
/*   520 */   722,  723,  724,  682,  701,  704,  705,  683,  690,  684,
/*   530 */   685,  692,  693,  694,  697,  698,  699,  700,  695,  696,
/*   540 */   866,  867,  870,  868,  687,  688,  702,  675,  664,  656,
/*   550 */   708,  711,  712,  713,  714,  715,  717,  709,  710,  654,
/*   560 */   645,  648,  757,  890,  899,  895,  891,  892,  893,  649,
/*   570 */   862,  863,  721,  794,  795,  889,  902,  904,  799,  905,
/*   580 */   907,  903,  932,  646,  725,  726,  729,  871,  914,  785,
/*   590 */   788,  791,  793,  873,  875,  877,  879,  880,  881,  882,
/*   600 */   883,  876,  878,  915,  919,  922,  924,  925,  926,  929,
/*   610 */   931,  936,  937,  938,  941,  942,  939,  626,  621,
};
    static int YY_SZ_ACTTAB = yy_action.Length;//(int)(yy_action.Length/sizeof(yy_action[0]))

    /* The next table maps tokens into fallback tokens.  If a construct
    ** like the following:
    **
    **      %fallback ID X Y Z.
    **
    ** appears in the grammar, then ID becomes a fallback token for X, Y,
    ** and Z.  Whenever one of the tokens X, Y, or Z is input to the parser
    ** but it does not parse, the type of the token is changed to ID and
    ** the parse is retried before an error is thrown.
    */
#if YYFALLBACK
    static YYCODETYPE[] yyFallback = new YYCODETYPE[]{
0,  /*          $ => nothing */
0,  /*       SEMI => nothing */
26,  /*    EXPLAIN => ID */
26,  /*      QUERY => ID */
26,  /*       PLAN => ID */
26,  /*      BEGIN => ID */
0,  /* TRANSACTION => nothing */
26,  /*   DEFERRED => ID */
26,  /*  IMMEDIATE => ID */
26,  /*  EXCLUSIVE => ID */
0,  /*     COMMIT => nothing */
26,  /*        END => ID */
26,  /*   ROLLBACK => ID */
26,  /*  SAVEPOINT => ID */
26,  /*    RELEASE => ID */
0,  /*         TO => nothing */
0,  /*      TABLE => nothing */
0,  /*     CREATE => nothing */
26,  /*         IF => ID */
0,  /*        NOT => nothing */
0,  /*     EXISTS => nothing */
26,  /*       TEMP => ID */
0,  /*         LP => nothing */
0,  /*         RP => nothing */
0,  /*         AS => nothing */
0,  /*      COMMA => nothing */
0,  /*         ID => nothing */
0,  /*    INDEXED => nothing */
26,  /*      ABORT => ID */
26,  /*      AFTER => ID */
26,  /*    ANALYZE => ID */
26,  /*        ASC => ID */
26,  /*     ATTACH => ID */
26,  /*     BEFORE => ID */
26,  /*         BY => ID */
26,  /*    CASCADE => ID */
26,  /*       CAST => ID */
26,  /*   COLUMNKW => ID */
26,  /*   CONFLICT => ID */
26,  /*   DATABASE => ID */
26,  /*       DESC => ID */
26,  /*     DETACH => ID */
26,  /*       EACH => ID */
26,  /*       FAIL => ID */
26,  /*        FOR => ID */
26,  /*     IGNORE => ID */
26,  /*  INITIALLY => ID */
26,  /*    INSTEAD => ID */
26,  /*    LIKE_KW => ID */
26,  /*      MATCH => ID */
26,  /*        KEY => ID */
26,  /*         OF => ID */
26,  /*     OFFSET => ID */
26,  /*     PRAGMA => ID */
26,  /*      RAISE => ID */
26,  /*    REPLACE => ID */
26,  /*   RESTRICT => ID */
26,  /*        ROW => ID */
26,  /*    TRIGGER => ID */
26,  /*     VACUUM => ID */
26,  /*       VIEW => ID */
26,  /*    VIRTUAL => ID */
26,  /*    REINDEX => ID */
26,  /*     RENAME => ID */
26,  /*   CTIME_KW => ID */
};
#endif // * YYFALLBACK */

    /* The following structure represents a single element of the
** parser's stack.  Information stored includes:
**
**   +  The state number for the parser at this level of the stack.
**
**   +  The value of the token stored at this level of the stack.
**      (In other words, the "major" token.)
**
**   +  The semantic value stored at this level of the stack.  This is
**      the information used by the action routines in the grammar.
**      It is sometimes called the "minor" token.
*/
    public class yyStackEntry
    {
      public YYACTIONTYPE stateno;       /* The state-number */
      public YYCODETYPE major;         /* The major token value.  This is the code
** number for the token at this stack level */
      public YYMINORTYPE minor; /* The user-supplied minor token value.  This
** is the value of the token  */
    };
    //typedef struct yyStackEntry yyStackEntry;

    /* The state of the parser is completely contained in an instance of
    ** the following structure */
    public class yyParser
    {
      public int yyidx;                    /* Index of top element in stack */
#if YYTRACKMAXSTACKDEPTH
int yyidxMax;                 /* Maximum value of yyidx */
#endif
      public int yyerrcnt;                 /* Shifts left before out of the error */
      public Parse pParse;  // sqlite3ParserARG_SDECL                /* A place to hold %extra_argument */
#if YYSTACKDEPTH//<=0
public int yystksz;                  /* Current side of the stack */
public yyStackEntry *yystack;        /* The parser's stack */
#else
      public yyStackEntry[] yystack = new yyStackEntry[YYSTACKDEPTH];  /* The parser's stack */
#endif
    };
    //typedef struct yyParser yyParser;

#if !NDEBUG
    //#include <stdio.h>
    static TextWriter yyTraceFILE = null;
    static string yyTracePrompt = "";
#endif // * NDEBUG */

#if !NDEBUG
    /*
** Turn parser tracing on by giving a stream to which to write the trace
** and a prompt to preface each trace message.  Tracing is turned off
** by making either argument NULL
**
** Inputs:
** <ul>
** <li> A FILE* to which trace output should be written.
**      If NULL, then tracing is turned off.
** <li> A prefix string written at the beginning of every
**      line of trace output.  If NULL, then tracing is
**      turned off.
** </ul>
**
** Outputs:
** None.
*/
    static void sqlite3ParserTrace( TextWriter TraceFILE, string zTracePrompt )
    {
      yyTraceFILE = TraceFILE;
      yyTracePrompt = zTracePrompt;
      if ( yyTraceFILE == null ) yyTracePrompt = "";
      else if ( yyTracePrompt == "" ) yyTraceFILE = null;
    }
#endif // * NDEBUG */

#if !NDEBUG
    /* For tracing shifts, the names of all terminals and nonterminals
** are required.  The following table supplies these names */
    static string[] yyTokenName = {
"$",             "SEMI",          "EXPLAIN",       "QUERY",
"PLAN",          "BEGIN",         "TRANSACTION",   "DEFERRED",
"IMMEDIATE",     "EXCLUSIVE",     "COMMIT",        "END",
"ROLLBACK",      "SAVEPOINT",     "RELEASE",       "TO",
"TABLE",         "CREATE",        "IF",            "NOT",
"EXISTS",        "TEMP",          "LP",            "RP",
"AS",            "COMMA",         "ID",            "INDEXED",
"ABORT",         "AFTER",         "ANALYZE",       "ASC",
"ATTACH",        "BEFORE",        "BY",            "CASCADE",
"CAST",          "COLUMNKW",      "CONFLICT",      "DATABASE",
"DESC",          "DETACH",        "EACH",          "FAIL",
"FOR",           "IGNORE",        "INITIALLY",     "INSTEAD",
"LIKE_KW",       "MATCH",         "KEY",           "OF",
"OFFSET",        "PRAGMA",        "RAISE",         "REPLACE",
"RESTRICT",      "ROW",           "TRIGGER",       "VACUUM",
"VIEW",          "VIRTUAL",       "REINDEX",       "RENAME",
"CTIME_KW",      "ANY",           "OR",            "AND",
"IS",            "BETWEEN",       "IN",            "ISNULL",
"NOTNULL",       "NE",            "EQ",            "GT",
"LE",            "LT",            "GE",            "ESCAPE",
"BITAND",        "BITOR",         "LSHIFT",        "RSHIFT",
"PLUS",          "MINUS",         "STAR",          "SLASH",
"REM",           "CONCAT",        "COLLATE",       "UMINUS",
"UPLUS",         "BITNOT",        "STRING",        "JOIN_KW",
"CONSTRAINT",    "DEFAULT",       "NULL",          "PRIMARY",
"UNIQUE",        "CHECK",         "REFERENCES",    "AUTOINCR",
"ON",            "DELETE",        "UPDATE",        "INSERT",
"SET",           "DEFERRABLE",    "FOREIGN",       "DROP",
"UNION",         "ALL",           "EXCEPT",        "INTERSECT",
"SELECT",        "DISTINCT",      "DOT",           "FROM",
"JOIN",          "USING",         "ORDER",         "GROUP",
"HAVING",        "LIMIT",         "WHERE",         "INTO",
"VALUES",        "INTEGER",       "FLOAT",         "BLOB",
"REGISTER",      "VARIABLE",      "CASE",          "WHEN",
"THEN",          "ELSE",          "INDEX",         "ALTER",
"ADD",           "error",         "input",         "cmdlist",
"ecmd",          "explain",       "cmdx",          "cmd",
"transtype",     "trans_opt",     "nm",            "savepoint_opt",
"create_table",  "create_table_args",  "createkw",      "temp",
"ifnotexists",   "dbnm",          "columnlist",    "conslist_opt",
"select",        "column",        "columnid",      "type",
"carglist",      "id",            "ids",           "typetoken",
"typename",      "signed",        "plus_num",      "minus_num",
"carg",          "ccons",         "term",          "expr",
"onconf",        "sortorder",     "autoinc",       "idxlist_opt",
"refargs",       "defer_subclause",  "refarg",        "refact",
"init_deferred_pred_opt",  "conslist",      "tcons",         "idxlist",
"defer_subclause_opt",  "orconf",        "resolvetype",   "raisetype",
"ifexists",      "fullname",      "oneselect",     "multiselect_op",
"distinct",      "selcollist",    "from",          "where_opt",
"groupby_opt",   "having_opt",    "orderby_opt",   "limit_opt",
"sclp",          "as",            "seltablist",    "stl_prefix",
"joinop",        "indexed_opt",   "on_opt",        "using_opt",
"joinop2",       "inscollist",    "sortlist",      "sortitem",
"nexprlist",     "setlist",       "insert_cmd",    "inscollist_opt",
"itemlist",      "exprlist",      "likeop",        "escape",
"between_op",    "in_op",         "case_operand",  "case_exprlist",
"case_else",     "uniqueflag",    "collate",       "nmnum",
"plus_opt",      "number",        "trigger_decl",  "trigger_cmd_list",
"trigger_time",  "trigger_event",  "foreach_clause",  "when_clause",
"trigger_cmd",   "database_kw_opt",  "key_opt",       "add_column_fullname",
"kwcolumn_opt",  "create_vtab",   "vtabarglist",   "vtabarg",
"vtabargtoken",  "lp",            "anylist",
};
#endif // * NDEBUG */

#if !NDEBUG
    /* For tracing reduce actions, the names of all rules are required.
*/
    static string[] yyRuleName = {
/*   0 */ "input ::= cmdlist",
/*   1 */ "cmdlist ::= cmdlist ecmd",
/*   2 */ "cmdlist ::= ecmd",
/*   3 */ "ecmd ::= SEMI",
/*   4 */ "ecmd ::= explain cmdx SEMI",
/*   5 */ "explain ::=",
/*   6 */ "explain ::= EXPLAIN",
/*   7 */ "explain ::= EXPLAIN QUERY PLAN",
/*   8 */ "cmdx ::= cmd",
/*   9 */ "cmd ::= BEGIN transtype trans_opt",
/*  10 */ "trans_opt ::=",
/*  11 */ "trans_opt ::= TRANSACTION",
/*  12 */ "trans_opt ::= TRANSACTION nm",
/*  13 */ "transtype ::=",
/*  14 */ "transtype ::= DEFERRED",
/*  15 */ "transtype ::= IMMEDIATE",
/*  16 */ "transtype ::= EXCLUSIVE",
/*  17 */ "cmd ::= COMMIT trans_opt",
/*  18 */ "cmd ::= END trans_opt",
/*  19 */ "cmd ::= ROLLBACK trans_opt",
/*  20 */ "savepoint_opt ::= SAVEPOINT",
/*  21 */ "savepoint_opt ::=",
/*  22 */ "cmd ::= SAVEPOINT nm",
/*  23 */ "cmd ::= RELEASE savepoint_opt nm",
/*  24 */ "cmd ::= ROLLBACK trans_opt TO savepoint_opt nm",
/*  25 */ "cmd ::= create_table create_table_args",
/*  26 */ "create_table ::= createkw temp TABLE ifnotexists nm dbnm",
/*  27 */ "createkw ::= CREATE",
/*  28 */ "ifnotexists ::=",
/*  29 */ "ifnotexists ::= IF NOT EXISTS",
/*  30 */ "temp ::= TEMP",
/*  31 */ "temp ::=",
/*  32 */ "create_table_args ::= LP columnlist conslist_opt RP",
/*  33 */ "create_table_args ::= AS select",
/*  34 */ "columnlist ::= columnlist COMMA column",
/*  35 */ "columnlist ::= column",
/*  36 */ "column ::= columnid type carglist",
/*  37 */ "columnid ::= nm",
/*  38 */ "id ::= ID",
/*  39 */ "id ::= INDEXED",
/*  40 */ "ids ::= ID|STRING",
/*  41 */ "nm ::= id",
/*  42 */ "nm ::= STRING",
/*  43 */ "nm ::= JOIN_KW",
/*  44 */ "type ::=",
/*  45 */ "type ::= typetoken",
/*  46 */ "typetoken ::= typename",
/*  47 */ "typetoken ::= typename LP signed RP",
/*  48 */ "typetoken ::= typename LP signed COMMA signed RP",
/*  49 */ "typename ::= ids",
/*  50 */ "typename ::= typename ids",
/*  51 */ "signed ::= plus_num",
/*  52 */ "signed ::= minus_num",
/*  53 */ "carglist ::= carglist carg",
/*  54 */ "carglist ::=",
/*  55 */ "carg ::= CONSTRAINT nm ccons",
/*  56 */ "carg ::= ccons",
/*  57 */ "ccons ::= DEFAULT term",
/*  58 */ "ccons ::= DEFAULT LP expr RP",
/*  59 */ "ccons ::= DEFAULT PLUS term",
/*  60 */ "ccons ::= DEFAULT MINUS term",
/*  61 */ "ccons ::= DEFAULT id",
/*  62 */ "ccons ::= NULL onconf",
/*  63 */ "ccons ::= NOT NULL onconf",
/*  64 */ "ccons ::= PRIMARY KEY sortorder onconf autoinc",
/*  65 */ "ccons ::= UNIQUE onconf",
/*  66 */ "ccons ::= CHECK LP expr RP",
/*  67 */ "ccons ::= REFERENCES nm idxlist_opt refargs",
/*  68 */ "ccons ::= defer_subclause",
/*  69 */ "ccons ::= COLLATE ids",
/*  70 */ "autoinc ::=",
/*  71 */ "autoinc ::= AUTOINCR",
/*  72 */ "refargs ::=",
/*  73 */ "refargs ::= refargs refarg",
/*  74 */ "refarg ::= MATCH nm",
/*  75 */ "refarg ::= ON DELETE refact",
/*  76 */ "refarg ::= ON UPDATE refact",
/*  77 */ "refarg ::= ON INSERT refact",
/*  78 */ "refact ::= SET NULL",
/*  79 */ "refact ::= SET DEFAULT",
/*  80 */ "refact ::= CASCADE",
/*  81 */ "refact ::= RESTRICT",
/*  82 */ "defer_subclause ::= NOT DEFERRABLE init_deferred_pred_opt",
/*  83 */ "defer_subclause ::= DEFERRABLE init_deferred_pred_opt",
/*  84 */ "init_deferred_pred_opt ::=",
/*  85 */ "init_deferred_pred_opt ::= INITIALLY DEFERRED",
/*  86 */ "init_deferred_pred_opt ::= INITIALLY IMMEDIATE",
/*  87 */ "conslist_opt ::=",
/*  88 */ "conslist_opt ::= COMMA conslist",
/*  89 */ "conslist ::= conslist COMMA tcons",
/*  90 */ "conslist ::= conslist tcons",
/*  91 */ "conslist ::= tcons",
/*  92 */ "tcons ::= CONSTRAINT nm",
/*  93 */ "tcons ::= PRIMARY KEY LP idxlist autoinc RP onconf",
/*  94 */ "tcons ::= UNIQUE LP idxlist RP onconf",
/*  95 */ "tcons ::= CHECK LP expr RP onconf",
/*  96 */ "tcons ::= FOREIGN KEY LP idxlist RP REFERENCES nm idxlist_opt refargs defer_subclause_opt",
/*  97 */ "defer_subclause_opt ::=",
/*  98 */ "defer_subclause_opt ::= defer_subclause",
/*  99 */ "onconf ::=",
/* 100 */ "onconf ::= ON CONFLICT resolvetype",
/* 101 */ "orconf ::=",
/* 102 */ "orconf ::= OR resolvetype",
/* 103 */ "resolvetype ::= raisetype",
/* 104 */ "resolvetype ::= IGNORE",
/* 105 */ "resolvetype ::= REPLACE",
/* 106 */ "cmd ::= DROP TABLE ifexists fullname",
/* 107 */ "ifexists ::= IF EXISTS",
/* 108 */ "ifexists ::=",
/* 109 */ "cmd ::= createkw temp VIEW ifnotexists nm dbnm AS select",
/* 110 */ "cmd ::= DROP VIEW ifexists fullname",
/* 111 */ "cmd ::= select",
/* 112 */ "select ::= oneselect",
/* 113 */ "select ::= select multiselect_op oneselect",
/* 114 */ "multiselect_op ::= UNION",
/* 115 */ "multiselect_op ::= UNION ALL",
/* 116 */ "multiselect_op ::= EXCEPT|INTERSECT",
/* 117 */ "oneselect ::= SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt",
/* 118 */ "distinct ::= DISTINCT",
/* 119 */ "distinct ::= ALL",
/* 120 */ "distinct ::=",
/* 121 */ "sclp ::= selcollist COMMA",
/* 122 */ "sclp ::=",
/* 123 */ "selcollist ::= sclp expr as",
/* 124 */ "selcollist ::= sclp STAR",
/* 125 */ "selcollist ::= sclp nm DOT STAR",
/* 126 */ "as ::= AS nm",
/* 127 */ "as ::= ids",
/* 128 */ "as ::=",
/* 129 */ "from ::=",
/* 130 */ "from ::= FROM seltablist",
/* 131 */ "stl_prefix ::= seltablist joinop",
/* 132 */ "stl_prefix ::=",
/* 133 */ "seltablist ::= stl_prefix nm dbnm as indexed_opt on_opt using_opt",
/* 134 */ "seltablist ::= stl_prefix LP select RP as on_opt using_opt",
/* 135 */ "seltablist ::= stl_prefix LP seltablist RP as on_opt using_opt",
/* 136 */ "dbnm ::=",
/* 137 */ "dbnm ::= DOT nm",
/* 138 */ "fullname ::= nm dbnm",
/* 139 */ "joinop ::= COMMA|JOIN",
/* 140 */ "joinop ::= JOIN_KW JOIN",
/* 141 */ "joinop ::= JOIN_KW nm JOIN",
/* 142 */ "joinop ::= JOIN_KW nm nm JOIN",
/* 143 */ "on_opt ::= ON expr",
/* 144 */ "on_opt ::=",
/* 145 */ "indexed_opt ::=",
/* 146 */ "indexed_opt ::= INDEXED BY nm",
/* 147 */ "indexed_opt ::= NOT INDEXED",
/* 148 */ "using_opt ::= USING LP inscollist RP",
/* 149 */ "using_opt ::=",
/* 150 */ "orderby_opt ::=",
/* 151 */ "orderby_opt ::= ORDER BY sortlist",
/* 152 */ "sortlist ::= sortlist COMMA sortitem sortorder",
/* 153 */ "sortlist ::= sortitem sortorder",
/* 154 */ "sortitem ::= expr",
/* 155 */ "sortorder ::= ASC",
/* 156 */ "sortorder ::= DESC",
/* 157 */ "sortorder ::=",
/* 158 */ "groupby_opt ::=",
/* 159 */ "groupby_opt ::= GROUP BY nexprlist",
/* 160 */ "having_opt ::=",
/* 161 */ "having_opt ::= HAVING expr",
/* 162 */ "limit_opt ::=",
/* 163 */ "limit_opt ::= LIMIT expr",
/* 164 */ "limit_opt ::= LIMIT expr OFFSET expr",
/* 165 */ "limit_opt ::= LIMIT expr COMMA expr",
/* 166 */ "cmd ::= DELETE FROM fullname indexed_opt where_opt",
/* 167 */ "where_opt ::=",
/* 168 */ "where_opt ::= WHERE expr",
/* 169 */ "cmd ::= UPDATE orconf fullname indexed_opt SET setlist where_opt",
/* 170 */ "setlist ::= setlist COMMA nm EQ expr",
/* 171 */ "setlist ::= nm EQ expr",
/* 172 */ "cmd ::= insert_cmd INTO fullname inscollist_opt VALUES LP itemlist RP",
/* 173 */ "cmd ::= insert_cmd INTO fullname inscollist_opt select",
/* 174 */ "cmd ::= insert_cmd INTO fullname inscollist_opt DEFAULT VALUES",
/* 175 */ "insert_cmd ::= INSERT orconf",
/* 176 */ "insert_cmd ::= REPLACE",
/* 177 */ "itemlist ::= itemlist COMMA expr",
/* 178 */ "itemlist ::= expr",
/* 179 */ "inscollist_opt ::=",
/* 180 */ "inscollist_opt ::= LP inscollist RP",
/* 181 */ "inscollist ::= inscollist COMMA nm",
/* 182 */ "inscollist ::= nm",
/* 183 */ "expr ::= term",
/* 184 */ "expr ::= LP expr RP",
/* 185 */ "term ::= NULL",
/* 186 */ "expr ::= id",
/* 187 */ "expr ::= JOIN_KW",
/* 188 */ "expr ::= nm DOT nm",
/* 189 */ "expr ::= nm DOT nm DOT nm",
/* 190 */ "term ::= INTEGER|FLOAT|BLOB",
/* 191 */ "term ::= STRING",
/* 192 */ "expr ::= REGISTER",
/* 193 */ "expr ::= VARIABLE",
/* 194 */ "expr ::= expr COLLATE ids",
/* 195 */ "expr ::= CAST LP expr AS typetoken RP",
/* 196 */ "expr ::= ID LP distinct exprlist RP",
/* 197 */ "expr ::= ID LP STAR RP",
/* 198 */ "term ::= CTIME_KW",
/* 199 */ "expr ::= expr AND expr",
/* 200 */ "expr ::= expr OR expr",
/* 201 */ "expr ::= expr LT|GT|GE|LE expr",
/* 202 */ "expr ::= expr EQ|NE expr",
/* 203 */ "expr ::= expr BITAND|BITOR|LSHIFT|RSHIFT expr",
/* 204 */ "expr ::= expr PLUS|MINUS expr",
/* 205 */ "expr ::= expr STAR|SLASH|REM expr",
/* 206 */ "expr ::= expr CONCAT expr",
/* 207 */ "likeop ::= LIKE_KW",
/* 208 */ "likeop ::= NOT LIKE_KW",
/* 209 */ "likeop ::= MATCH",
/* 210 */ "likeop ::= NOT MATCH",
/* 211 */ "escape ::= ESCAPE expr",
/* 212 */ "escape ::=",
/* 213 */ "expr ::= expr likeop expr escape",
/* 214 */ "expr ::= expr ISNULL|NOTNULL",
/* 215 */ "expr ::= expr IS NULL",
/* 216 */ "expr ::= expr NOT NULL",
/* 217 */ "expr ::= expr IS NOT NULL",
/* 218 */ "expr ::= NOT expr",
/* 219 */ "expr ::= BITNOT expr",
/* 220 */ "expr ::= MINUS expr",
/* 221 */ "expr ::= PLUS expr",
/* 222 */ "between_op ::= BETWEEN",
/* 223 */ "between_op ::= NOT BETWEEN",
/* 224 */ "expr ::= expr between_op expr AND expr",
/* 225 */ "in_op ::= IN",
/* 226 */ "in_op ::= NOT IN",
/* 227 */ "expr ::= expr in_op LP exprlist RP",
/* 228 */ "expr ::= LP select RP",
/* 229 */ "expr ::= expr in_op LP select RP",
/* 230 */ "expr ::= expr in_op nm dbnm",
/* 231 */ "expr ::= EXISTS LP select RP",
/* 232 */ "expr ::= CASE case_operand case_exprlist case_else END",
/* 233 */ "case_exprlist ::= case_exprlist WHEN expr THEN expr",
/* 234 */ "case_exprlist ::= WHEN expr THEN expr",
/* 235 */ "case_else ::= ELSE expr",
/* 236 */ "case_else ::=",
/* 237 */ "case_operand ::= expr",
/* 238 */ "case_operand ::=",
/* 239 */ "exprlist ::= nexprlist",
/* 240 */ "exprlist ::=",
/* 241 */ "nexprlist ::= nexprlist COMMA expr",
/* 242 */ "nexprlist ::= expr",
/* 243 */ "cmd ::= createkw uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP",
/* 244 */ "uniqueflag ::= UNIQUE",
/* 245 */ "uniqueflag ::=",
/* 246 */ "idxlist_opt ::=",
/* 247 */ "idxlist_opt ::= LP idxlist RP",
/* 248 */ "idxlist ::= idxlist COMMA nm collate sortorder",
/* 249 */ "idxlist ::= nm collate sortorder",
/* 250 */ "collate ::=",
/* 251 */ "collate ::= COLLATE ids",
/* 252 */ "cmd ::= DROP INDEX ifexists fullname",
/* 253 */ "cmd ::= VACUUM",
/* 254 */ "cmd ::= VACUUM nm",
/* 255 */ "cmd ::= PRAGMA nm dbnm",
/* 256 */ "cmd ::= PRAGMA nm dbnm EQ nmnum",
/* 257 */ "cmd ::= PRAGMA nm dbnm LP nmnum RP",
/* 258 */ "cmd ::= PRAGMA nm dbnm EQ minus_num",
/* 259 */ "cmd ::= PRAGMA nm dbnm LP minus_num RP",
/* 260 */ "nmnum ::= plus_num",
/* 261 */ "nmnum ::= nm",
/* 262 */ "nmnum ::= ON",
/* 263 */ "nmnum ::= DELETE",
/* 264 */ "nmnum ::= DEFAULT",
/* 265 */ "plus_num ::= plus_opt number",
/* 266 */ "minus_num ::= MINUS number",
/* 267 */ "number ::= INTEGER|FLOAT",
/* 268 */ "plus_opt ::= PLUS",
/* 269 */ "plus_opt ::=",
/* 270 */ "cmd ::= createkw trigger_decl BEGIN trigger_cmd_list END",
/* 271 */ "trigger_decl ::= temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event ON fullname foreach_clause when_clause",
/* 272 */ "trigger_time ::= BEFORE",
/* 273 */ "trigger_time ::= AFTER",
/* 274 */ "trigger_time ::= INSTEAD OF",
/* 275 */ "trigger_time ::=",
/* 276 */ "trigger_event ::= DELETE|INSERT",
/* 277 */ "trigger_event ::= UPDATE",
/* 278 */ "trigger_event ::= UPDATE OF inscollist",
/* 279 */ "foreach_clause ::=",
/* 280 */ "foreach_clause ::= FOR EACH ROW",
/* 281 */ "when_clause ::=",
/* 282 */ "when_clause ::= WHEN expr",
/* 283 */ "trigger_cmd_list ::= trigger_cmd_list trigger_cmd SEMI",
/* 284 */ "trigger_cmd_list ::= trigger_cmd SEMI",
/* 285 */ "trigger_cmd ::= UPDATE orconf nm SET setlist where_opt",
/* 286 */ "trigger_cmd ::= insert_cmd INTO nm inscollist_opt VALUES LP itemlist RP",
/* 287 */ "trigger_cmd ::= insert_cmd INTO nm inscollist_opt select",
/* 288 */ "trigger_cmd ::= DELETE FROM nm where_opt",
/* 289 */ "trigger_cmd ::= select",
/* 290 */ "expr ::= RAISE LP IGNORE RP",
/* 291 */ "expr ::= RAISE LP raisetype COMMA nm RP",
/* 292 */ "raisetype ::= ROLLBACK",
/* 293 */ "raisetype ::= ABORT",
/* 294 */ "raisetype ::= FAIL",
/* 295 */ "cmd ::= DROP TRIGGER ifexists fullname",
/* 296 */ "cmd ::= ATTACH database_kw_opt expr AS expr key_opt",
/* 297 */ "cmd ::= DETACH database_kw_opt expr",
/* 298 */ "key_opt ::=",
/* 299 */ "key_opt ::= KEY expr",
/* 300 */ "database_kw_opt ::= DATABASE",
/* 301 */ "database_kw_opt ::=",
/* 302 */ "cmd ::= REINDEX",
/* 303 */ "cmd ::= REINDEX nm dbnm",
/* 304 */ "cmd ::= ANALYZE",
/* 305 */ "cmd ::= ANALYZE nm dbnm",
/* 306 */ "cmd ::= ALTER TABLE fullname RENAME TO nm",
/* 307 */ "cmd ::= ALTER TABLE add_column_fullname ADD kwcolumn_opt column",
/* 308 */ "add_column_fullname ::= fullname",
/* 309 */ "kwcolumn_opt ::=",
/* 310 */ "kwcolumn_opt ::= COLUMNKW",
/* 311 */ "cmd ::= create_vtab",
/* 312 */ "cmd ::= create_vtab LP vtabarglist RP",
/* 313 */ "create_vtab ::= createkw VIRTUAL TABLE nm dbnm USING nm",
/* 314 */ "vtabarglist ::= vtabarg",
/* 315 */ "vtabarglist ::= vtabarglist COMMA vtabarg",
/* 316 */ "vtabarg ::=",
/* 317 */ "vtabarg ::= vtabarg vtabargtoken",
/* 318 */ "vtabargtoken ::= ANY",
/* 319 */ "vtabargtoken ::= lp anylist RP",
/* 320 */ "lp ::= LP",
/* 321 */ "anylist ::=",
/* 322 */ "anylist ::= anylist LP anylist RP",
/* 323 */ "anylist ::= anylist ANY",
};
#endif // * NDEBUG */


#if YYSTACKDEPTH//<=0
/*
** Try to increase the size of the parser stack.
*/
static void yyGrowStack(yyParser p){
int newSize;
//yyStackEntry pNew;

newSize = p.yystksz*2 + 100;
//pNew = realloc(p.yystack, newSize*sizeof(pNew[0]));
//if( pNew !=null){
p.yystack = Array.Resize(p.yystack,newSize); //pNew;
p.yystksz = newSize;
#if !NDEBUG
if( yyTraceFILE ){
fprintf(yyTraceFILE,"%sStack grows to %d entries!\n",
yyTracePrompt, p.yystksz);
}
#endif
//}
}
#endif

    /*
** This function allocates a new parser.
** The only argument is a pointer to a function which works like
** malloc.
**
** Inputs:
** A pointer to the function used to allocate memory.
**
** Outputs:
** A pointer to a parser.  This pointer is used in subsequent calls
** to sqlite3Parser and sqlite3ParserFree.
*/
    static yyParser sqlite3ParserAlloc()
    {//void *(*mallocProc)(size_t)){
      yyParser pParser = new yyParser();
      //pParser = (yyParser*)(*mallocProc)( (size_t)yyParser.Length );
      if ( pParser != null )
      {
        pParser.yyidx = -1;
#if YYTRACKMAXSTACKDEPTH
pParser.yyidxMax=0;
#endif

#if YYSTACKDEPTH//<=0
pParser.yystack = NULL;
pParser.yystksz = 0;
yyGrowStack(pParser);
#endif
      }
      return pParser;
    }

    /* The following function deletes the value associated with a
    ** symbol.  The symbol can be either a terminal or nonterminal.
    ** "yymajor" is the symbol code, and "yypminor" is a pointer to
    ** the value.
    */
    static void yy_destructor(
    yyParser yypParser,    /* The parser */
    YYCODETYPE yymajor,    /* Type code for object to destroy */
    YYMINORTYPE yypminor   /* The object to be destroyed */
    )
    {
      Parse pParse = yypParser.pParse; // sqlite3ParserARG_FETCH;
      switch ( yymajor )
      {
        /* Here is inserted the actions which take place when a
        ** terminal or non-terminal is destroyed.  This can happen
        ** when the symbol is popped from the stack during a
        ** reduce or during error processing or when a parser is
        ** being destroyed before it is finished parsing.
        **
        ** Note: during a reduce, the only symbols destroyed are those
        ** which appear on the RHS of the rule, but which are not used
        ** inside the C code.
        */
        case 160: /* select */
        case 194: /* oneselect */
          {
            //#line 404 "parse.y"
            sqlite3SelectDelete( pParse.db, ref ( yypminor.yy243 ) );
            //#line 1356 "parse.c"
          }
          break;
        case 174: /* term */
        case 175: /* expr */
        case 223: /* escape */
          {
            //#line 723 "parse.y"
            sqlite3ExprDelete( pParse.db, ref ( yypminor.yy190 ).pExpr );
            //#line 1365 "parse.c"
          }
          break;
        case 179: /* idxlist_opt */
        case 187: /* idxlist */
        case 197: /* selcollist */
        case 200: /* groupby_opt */
        case 202: /* orderby_opt */
        case 204: /* sclp */
        case 214: /* sortlist */
        case 216: /* nexprlist */
        case 217: /* setlist */
        case 220: /* itemlist */
        case 221: /* exprlist */
        case 227: /* case_exprlist */
          {
            //#line 1064 "parse.y"
            sqlite3ExprListDelete( pParse.db, ref  ( yypminor.yy148 ) );
            //#line 1383 "parse.c"
          }
          break;
        case 193: /* fullname */
        case 198: /* from */
        case 206: /* seltablist */
        case 207: /* stl_prefix */
          {
            //#line 537 "parse.y"
            sqlite3SrcListDelete( pParse.db, ref  ( yypminor.yy185 ) );
            //#line 1393 "parse.c"
          }
          break;
        case 199: /* where_opt */
        case 201: /* having_opt */
        case 210: /* on_opt */
        case 215: /* sortitem */
        case 226: /* case_operand */
        case 228: /* case_else */
        case 239: /* when_clause */
        case 242: /* key_opt */
          {
            //#line 647 "parse.y"
            sqlite3ExprDelete( pParse.db, ref ( yypminor.yy72 ) );
            //#line 1407 "parse.c"
          }
          break;
        case 211: /* using_opt */
        case 213: /* inscollist */
        case 219: /* inscollist_opt */
          {
            //#line 569 "parse.y"
            sqlite3IdListDelete( pParse.db, ref ( yypminor.yy254 ) );
            //#line 1416 "parse.c"
          }
          break;
        case 235: /* trigger_cmd_list */
        case 240: /* trigger_cmd */
          {
            //#line 1171 "parse.y"
            sqlite3DeleteTriggerStep( pParse.db, ref ( yypminor.yy145 ) );
            //#line 1424 "parse.c"
          }
          break;
        case 237: /* trigger_event */
          {
            //#line 1157 "parse.y"
            sqlite3IdListDelete( pParse.db, ref ( yypminor.yy332 ).b );
            //#line 1431 "parse.c"
          }
          break;
        default: break;   /* If no destructor action specified: do nothing */
      }
    }

    /*
    ** Pop the parser's stack once.
    **
    ** If there is a destructor routine associated with the token which
    ** is popped from the stack, then call it.
    **
    ** Return the major token number for the symbol popped.
    */
    static int yy_pop_parser_stack( yyParser pParser )
    {
      YYCODETYPE yymajor;
      yyStackEntry yytos = pParser.yystack[pParser.yyidx];

      if ( pParser.yyidx < 0 ) return 0;
#if !NDEBUG
      if ( yyTraceFILE != null && pParser.yyidx >= 0 )
      {
        fprintf( yyTraceFILE, "%sPopping %s\n",
        yyTracePrompt,
        yyTokenName[yytos.major] );
      }
#endif
      yymajor = yytos.major;
      yy_destructor( pParser, yymajor, yytos.minor );
      pParser.yyidx--;
      return yymajor;
    }

    /*
    ** Deallocate and destroy a parser.  Destructors are all called for
    ** all stack elements before shutting the parser down.
    **
    ** Inputs:
    ** <ul>
    ** <li>  A pointer to the parser.  This should be a pointer
    **       obtained from sqlite3ParserAlloc.
    ** <li>  A pointer to a function used to reclaim memory obtained
    **       from malloc.
    ** </ul>
    */
    static void sqlite3ParserFree(
    yyParser p,                    /* The parser to be deleted */
    dxDel freeProc//)(void*)     /* Function used to reclaim memory */
    )
    {
      yyParser pParser = p;
      if ( pParser == null ) return;
      while ( pParser.yyidx >= 0 ) yy_pop_parser_stack( pParser );
#if YYSTACKDEPTH//<=0
pParser.yystack = null;//free(pParser.yystack);
#endif
      pParser = null;// freeProc(ref pParser);
    }

    /*
    ** Return the peak depth of the stack for a parser.
    */
#if YYTRACKMAXSTACKDEPTH
int sqlite3ParserStackPeak(void p){
yyParser pParser = (yyParser*)p;
return pParser.yyidxMax;
}
#endif

    /*
** Find the appropriate action for a parser given the terminal
** look-ahead token iLookAhead.
**
** If the look-ahead token is YYNOCODE, then check to see if the action is
** independent of the look-ahead.  If it is, return the action, otherwise
** return YY_NO_ACTION.
*/
    static int yy_find_shift_action(
    yyParser pParser,         /* The parser */
    YYCODETYPE iLookAhead     /* The look-ahead token */
    )
    {
      int i;
      int stateno = pParser.yystack[pParser.yyidx].stateno;

      if ( stateno > YY_SHIFT_MAX || ( i = yy_shift_ofst[stateno] ) == YY_SHIFT_USE_DFLT )
      {
        return yy_default[stateno];
      }
      Debug.Assert( iLookAhead != YYNOCODE );
      i += iLookAhead;
      if ( i < 0 || i >= YY_SZ_ACTTAB || yy_lookahead[i] != iLookAhead )
      {
        if ( iLookAhead > 0 )
        {
#if YYFALLBACK
          YYCODETYPE iFallback;            /* Fallback token */
          if ( iLookAhead < yyFallback.Length //yyFallback.Length/sizeof(yyFallback[0])
          && ( iFallback = yyFallback[iLookAhead] ) != 0 )
          {
#if !NDEBUG
            if ( yyTraceFILE != null )
            {
              fprintf( yyTraceFILE, "%sFALLBACK %s => %s\n",
              yyTracePrompt, yyTokenName[iLookAhead], yyTokenName[iFallback] );
            }
#endif
            return yy_find_shift_action( pParser, iFallback );
          }
#endif
#if YYWILDCARD
          {
            int j = i - iLookAhead + YYWILDCARD;
            if ( j >= 0 && j < YY_SZ_ACTTAB && yy_lookahead[j] == YYWILDCARD )
            {
#if !NDEBUG
              if ( yyTraceFILE != null )
              {
                Debugger.Break(); // TODO --
                //fprintf(yyTraceFILE, "%sWILDCARD %s => %s\n",
                //   yyTracePrompt, yyTokenName[iLookAhead], yyTokenName[YYWILDCARD]);
              }
#endif // * NDEBUG */
              return yy_action[j];
            }
          }
#endif // * YYWILDCARD */
        }
        return yy_default[stateno];
      }
      else
      {
        return yy_action[i];
      }
    }

    /*
    ** Find the appropriate action for a parser given the non-terminal
    ** look-ahead token iLookAhead.
    **
    ** If the look-ahead token is YYNOCODE, then check to see if the action is
    ** independent of the look-ahead.  If it is, return the action, otherwise
    ** return YY_NO_ACTION.
    */
    static int yy_find_reduce_action(
    int stateno,              /* Current state number */
    YYCODETYPE iLookAhead     /* The look-ahead token */
    )
    {
      int i;
#if YYERRORSYMBOL
if( stateno>YY_REDUCE_MAX ){
return yy_default[stateno];
}
#else
      Debug.Assert( stateno <= YY_REDUCE_MAX );
#endif
      i = yy_reduce_ofst[stateno];
      Debug.Assert( i != YY_REDUCE_USE_DFLT );
      Debug.Assert( iLookAhead != YYNOCODE );
      i += iLookAhead;
#if YYERRORSYMBOL
if( i<0 || i>=YY_SZ_ACTTAB || yy_lookahead[i]!=iLookAhead ){
return yy_default[stateno];
}
#else
      Debug.Assert( i >= 0 && i < YY_SZ_ACTTAB );
      Debug.Assert( yy_lookahead[i] == iLookAhead );
#endif
      return yy_action[i];
    }

    /*
    ** The following routine is called if the stack overflows.
    */
    static void yyStackOverflow( yyParser yypParser, YYMINORTYPE yypMinor )
    {
      Parse pParse = yypParser.pParse; // sqlite3ParserARG_FETCH;
      yypParser.yyidx--;
#if !NDEBUG
      if ( yyTraceFILE != null )
      {
        Debugger.Break(); // TODO --
        //fprintf(yyTraceFILE, "%sStack Overflow!\n", yyTracePrompt);
      }
#endif
      while ( yypParser.yyidx >= 0 ) yy_pop_parser_stack( yypParser );
      /* Here code is inserted which will execute if the parser
      ** stack every overflows */
      //#line 40 "parse.y"

      UNUSED_PARAMETER( yypMinor ); /* Silence some compiler warnings */
      sqlite3ErrorMsg( pParse, "parser stack overflow" );
      pParse.parseError = 1;
      //#line 1609 "parse.c"
      yypParser.pParse = pParse;//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument var */
    }

    /*
    ** Perform a shift action.
    */
    static void yy_shift(
    yyParser yypParser,          /* The parser to be shifted */
    int yyNewState,               /* The new state to shift in */
    int yyMajor,                  /* The major token to shift in */
    YYMINORTYPE yypMinor         /* Pointer to the minor token to shift in */
    )
    {
      yyStackEntry yytos = new yyStackEntry();
      yypParser.yyidx++;
#if YYTRACKMAXSTACKDEPTH
if( yypParser.yyidx>yypParser.yyidxMax ){
yypParser.yyidxMax = yypParser.yyidx;
}
#endif
#if !YYSTACKDEPTH//was YYSTACKDEPTH>0
      if ( yypParser.yyidx >= YYSTACKDEPTH )
      {
        yyStackOverflow( yypParser, yypMinor );
        return;
      }
#else
if( yypParser.yyidx>=yypParser.yystksz ){
yyGrowStack(yypParser);
if( yypParser.yyidx>=yypParser.yystksz ){
yyStackOverflow(yypParser, yypMinor);
return;
}
}
#endif
      yypParser.yystack[yypParser.yyidx] = yytos;//yytos = yypParser.yystack[yypParser.yyidx];
      yytos.stateno = (YYACTIONTYPE)yyNewState;
      yytos.major = (YYCODETYPE)yyMajor;
      yytos.minor = yypMinor;
#if !NDEBUG
      if ( yyTraceFILE != null && yypParser.yyidx > 0 )
      {
        int i;
        fprintf( yyTraceFILE, "%sShift %d\n", yyTracePrompt, yyNewState );
        fprintf( yyTraceFILE, "%sStack:", yyTracePrompt );
        for ( i = 1 ; i <= yypParser.yyidx ; i++ )
          fprintf( yyTraceFILE, " %s", yyTokenName[yypParser.yystack[i].major] );
        fprintf( yyTraceFILE, "\n" );
      }
#endif
    }
    /* The following table contains information about every rule that
    ** is used during the reduce.
    */
    public struct _yyRuleInfo
    {
      public YYCODETYPE lhs;         /* Symbol on the left-hand side of the rule */
      public byte nrhs;     /* Number of right-hand side symbols in the rule */
      public _yyRuleInfo( YYCODETYPE lhs, byte nrhs )
      {
        this.lhs = lhs;
        this.nrhs = nrhs;
      }

    }
    static _yyRuleInfo[] yyRuleInfo = new _yyRuleInfo[]{
new _yyRuleInfo( 142, 1 ),
new _yyRuleInfo( 143, 2 ),
new _yyRuleInfo( 143, 1 ),
new _yyRuleInfo( 144, 1 ),
new _yyRuleInfo( 144, 3 ),
new _yyRuleInfo( 145, 0 ),
new _yyRuleInfo( 145, 1 ),
new _yyRuleInfo( 145, 3 ),
new _yyRuleInfo( 146, 1 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 149, 0 ),
new _yyRuleInfo( 149, 1 ),
new _yyRuleInfo( 149, 2 ),
new _yyRuleInfo( 148, 0 ),
new _yyRuleInfo( 148, 1 ),
new _yyRuleInfo( 148, 1 ),
new _yyRuleInfo( 148, 1 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 151, 1 ),
new _yyRuleInfo( 151, 0 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 152, 6 ),
new _yyRuleInfo( 154, 1 ),
new _yyRuleInfo( 156, 0 ),
new _yyRuleInfo( 156, 3 ),
new _yyRuleInfo( 155, 1 ),
new _yyRuleInfo( 155, 0 ),
new _yyRuleInfo( 153, 4 ),
new _yyRuleInfo( 153, 2 ),
new _yyRuleInfo( 158, 3 ),
new _yyRuleInfo( 158, 1 ),
new _yyRuleInfo( 161, 3 ),
new _yyRuleInfo( 162, 1 ),
new _yyRuleInfo( 165, 1 ),
new _yyRuleInfo( 165, 1 ),
new _yyRuleInfo( 166, 1 ),
new _yyRuleInfo( 150, 1 ),
new _yyRuleInfo( 150, 1 ),
new _yyRuleInfo( 150, 1 ),
new _yyRuleInfo( 163, 0 ),
new _yyRuleInfo( 163, 1 ),
new _yyRuleInfo( 167, 1 ),
new _yyRuleInfo( 167, 4 ),
new _yyRuleInfo( 167, 6 ),
new _yyRuleInfo( 168, 1 ),
new _yyRuleInfo( 168, 2 ),
new _yyRuleInfo( 169, 1 ),
new _yyRuleInfo( 169, 1 ),
new _yyRuleInfo( 164, 2 ),
new _yyRuleInfo( 164, 0 ),
new _yyRuleInfo( 172, 3 ),
new _yyRuleInfo( 172, 1 ),
new _yyRuleInfo( 173, 2 ),
new _yyRuleInfo( 173, 4 ),
new _yyRuleInfo( 173, 3 ),
new _yyRuleInfo( 173, 3 ),
new _yyRuleInfo( 173, 2 ),
new _yyRuleInfo( 173, 2 ),
new _yyRuleInfo( 173, 3 ),
new _yyRuleInfo( 173, 5 ),
new _yyRuleInfo( 173, 2 ),
new _yyRuleInfo( 173, 4 ),
new _yyRuleInfo( 173, 4 ),
new _yyRuleInfo( 173, 1 ),
new _yyRuleInfo( 173, 2 ),
new _yyRuleInfo( 178, 0 ),
new _yyRuleInfo( 178, 1 ),
new _yyRuleInfo( 180, 0 ),
new _yyRuleInfo( 180, 2 ),
new _yyRuleInfo( 182, 2 ),
new _yyRuleInfo( 182, 3 ),
new _yyRuleInfo( 182, 3 ),
new _yyRuleInfo( 182, 3 ),
new _yyRuleInfo( 183, 2 ),
new _yyRuleInfo( 183, 2 ),
new _yyRuleInfo( 183, 1 ),
new _yyRuleInfo( 183, 1 ),
new _yyRuleInfo( 181, 3 ),
new _yyRuleInfo( 181, 2 ),
new _yyRuleInfo( 184, 0 ),
new _yyRuleInfo( 184, 2 ),
new _yyRuleInfo( 184, 2 ),
new _yyRuleInfo( 159, 0 ),
new _yyRuleInfo( 159, 2 ),
new _yyRuleInfo( 185, 3 ),
new _yyRuleInfo( 185, 2 ),
new _yyRuleInfo( 185, 1 ),
new _yyRuleInfo( 186, 2 ),
new _yyRuleInfo( 186, 7 ),
new _yyRuleInfo( 186, 5 ),
new _yyRuleInfo( 186, 5 ),
new _yyRuleInfo( 186, 10 ),
new _yyRuleInfo( 188, 0 ),
new _yyRuleInfo( 188, 1 ),
new _yyRuleInfo( 176, 0 ),
new _yyRuleInfo( 176, 3 ),
new _yyRuleInfo( 189, 0 ),
new _yyRuleInfo( 189, 2 ),
new _yyRuleInfo( 190, 1 ),
new _yyRuleInfo( 190, 1 ),
new _yyRuleInfo( 190, 1 ),
new _yyRuleInfo( 147, 4 ),
new _yyRuleInfo( 192, 2 ),
new _yyRuleInfo( 192, 0 ),
new _yyRuleInfo( 147, 8 ),
new _yyRuleInfo( 147, 4 ),
new _yyRuleInfo( 147, 1 ),
new _yyRuleInfo( 160, 1 ),
new _yyRuleInfo( 160, 3 ),
new _yyRuleInfo( 195, 1 ),
new _yyRuleInfo( 195, 2 ),
new _yyRuleInfo( 195, 1 ),
new _yyRuleInfo( 194, 9 ),
new _yyRuleInfo( 196, 1 ),
new _yyRuleInfo( 196, 1 ),
new _yyRuleInfo( 196, 0 ),
new _yyRuleInfo( 204, 2 ),
new _yyRuleInfo( 204, 0 ),
new _yyRuleInfo( 197, 3 ),
new _yyRuleInfo( 197, 2 ),
new _yyRuleInfo( 197, 4 ),
new _yyRuleInfo( 205, 2 ),
new _yyRuleInfo( 205, 1 ),
new _yyRuleInfo( 205, 0 ),
new _yyRuleInfo( 198, 0 ),
new _yyRuleInfo( 198, 2 ),
new _yyRuleInfo( 207, 2 ),
new _yyRuleInfo( 207, 0 ),
new _yyRuleInfo( 206, 7 ),
new _yyRuleInfo( 206, 7 ),
new _yyRuleInfo( 206, 7 ),
new _yyRuleInfo( 157, 0 ),
new _yyRuleInfo( 157, 2 ),
new _yyRuleInfo( 193, 2 ),
new _yyRuleInfo( 208, 1 ),
new _yyRuleInfo( 208, 2 ),
new _yyRuleInfo( 208, 3 ),
new _yyRuleInfo( 208, 4 ),
new _yyRuleInfo( 210, 2 ),
new _yyRuleInfo( 210, 0 ),
new _yyRuleInfo( 209, 0 ),
new _yyRuleInfo( 209, 3 ),
new _yyRuleInfo( 209, 2 ),
new _yyRuleInfo( 211, 4 ),
new _yyRuleInfo( 211, 0 ),
new _yyRuleInfo( 202, 0 ),
new _yyRuleInfo( 202, 3 ),
new _yyRuleInfo( 214, 4 ),
new _yyRuleInfo( 214, 2 ),
new _yyRuleInfo( 215, 1 ),
new _yyRuleInfo( 177, 1 ),
new _yyRuleInfo( 177, 1 ),
new _yyRuleInfo( 177, 0 ),
new _yyRuleInfo( 200, 0 ),
new _yyRuleInfo( 200, 3 ),
new _yyRuleInfo( 201, 0 ),
new _yyRuleInfo( 201, 2 ),
new _yyRuleInfo( 203, 0 ),
new _yyRuleInfo( 203, 2 ),
new _yyRuleInfo( 203, 4 ),
new _yyRuleInfo( 203, 4 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 199, 0 ),
new _yyRuleInfo( 199, 2 ),
new _yyRuleInfo( 147, 7 ),
new _yyRuleInfo( 217, 5 ),
new _yyRuleInfo( 217, 3 ),
new _yyRuleInfo( 147, 8 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 218, 2 ),
new _yyRuleInfo( 218, 1 ),
new _yyRuleInfo( 220, 3 ),
new _yyRuleInfo( 220, 1 ),
new _yyRuleInfo( 219, 0 ),
new _yyRuleInfo( 219, 3 ),
new _yyRuleInfo( 213, 3 ),
new _yyRuleInfo( 213, 1 ),
new _yyRuleInfo( 175, 1 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 174, 1 ),
new _yyRuleInfo( 175, 1 ),
new _yyRuleInfo( 175, 1 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 174, 1 ),
new _yyRuleInfo( 174, 1 ),
new _yyRuleInfo( 175, 1 ),
new _yyRuleInfo( 175, 1 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 6 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 174, 1 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 222, 1 ),
new _yyRuleInfo( 222, 2 ),
new _yyRuleInfo( 222, 1 ),
new _yyRuleInfo( 222, 2 ),
new _yyRuleInfo( 223, 2 ),
new _yyRuleInfo( 223, 0 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 175, 2 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 175, 2 ),
new _yyRuleInfo( 175, 2 ),
new _yyRuleInfo( 175, 2 ),
new _yyRuleInfo( 175, 2 ),
new _yyRuleInfo( 224, 1 ),
new _yyRuleInfo( 224, 2 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 225, 1 ),
new _yyRuleInfo( 225, 2 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 175, 3 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 175, 5 ),
new _yyRuleInfo( 227, 5 ),
new _yyRuleInfo( 227, 4 ),
new _yyRuleInfo( 228, 2 ),
new _yyRuleInfo( 228, 0 ),
new _yyRuleInfo( 226, 1 ),
new _yyRuleInfo( 226, 0 ),
new _yyRuleInfo( 221, 1 ),
new _yyRuleInfo( 221, 0 ),
new _yyRuleInfo( 216, 3 ),
new _yyRuleInfo( 216, 1 ),
new _yyRuleInfo( 147, 11 ),
new _yyRuleInfo( 229, 1 ),
new _yyRuleInfo( 229, 0 ),
new _yyRuleInfo( 179, 0 ),
new _yyRuleInfo( 179, 3 ),
new _yyRuleInfo( 187, 5 ),
new _yyRuleInfo( 187, 3 ),
new _yyRuleInfo( 230, 0 ),
new _yyRuleInfo( 230, 2 ),
new _yyRuleInfo( 147, 4 ),
new _yyRuleInfo( 147, 1 ),
new _yyRuleInfo( 147, 2 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 231, 1 ),
new _yyRuleInfo( 231, 1 ),
new _yyRuleInfo( 231, 1 ),
new _yyRuleInfo( 231, 1 ),
new _yyRuleInfo( 231, 1 ),
new _yyRuleInfo( 170, 2 ),
new _yyRuleInfo( 171, 2 ),
new _yyRuleInfo( 233, 1 ),
new _yyRuleInfo( 232, 1 ),
new _yyRuleInfo( 232, 0 ),
new _yyRuleInfo( 147, 5 ),
new _yyRuleInfo( 234, 11 ),
new _yyRuleInfo( 236, 1 ),
new _yyRuleInfo( 236, 1 ),
new _yyRuleInfo( 236, 2 ),
new _yyRuleInfo( 236, 0 ),
new _yyRuleInfo( 237, 1 ),
new _yyRuleInfo( 237, 1 ),
new _yyRuleInfo( 237, 3 ),
new _yyRuleInfo( 238, 0 ),
new _yyRuleInfo( 238, 3 ),
new _yyRuleInfo( 239, 0 ),
new _yyRuleInfo( 239, 2 ),
new _yyRuleInfo( 235, 3 ),
new _yyRuleInfo( 235, 2 ),
new _yyRuleInfo( 240, 6 ),
new _yyRuleInfo( 240, 8 ),
new _yyRuleInfo( 240, 5 ),
new _yyRuleInfo( 240, 4 ),
new _yyRuleInfo( 240, 1 ),
new _yyRuleInfo( 175, 4 ),
new _yyRuleInfo( 175, 6 ),
new _yyRuleInfo( 191, 1 ),
new _yyRuleInfo( 191, 1 ),
new _yyRuleInfo( 191, 1 ),
new _yyRuleInfo( 147, 4 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 242, 0 ),
new _yyRuleInfo( 242, 2 ),
new _yyRuleInfo( 241, 1 ),
new _yyRuleInfo( 241, 0 ),
new _yyRuleInfo( 147, 1 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 147, 1 ),
new _yyRuleInfo( 147, 3 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 147, 6 ),
new _yyRuleInfo( 243, 1 ),
new _yyRuleInfo( 244, 0 ),
new _yyRuleInfo( 244, 1 ),
new _yyRuleInfo( 147, 1 ),
new _yyRuleInfo( 147, 4 ),
new _yyRuleInfo( 245, 7 ),
new _yyRuleInfo( 246, 1 ),
new _yyRuleInfo( 246, 3 ),
new _yyRuleInfo( 247, 0 ),
new _yyRuleInfo( 247, 2 ),
new _yyRuleInfo( 248, 1 ),
new _yyRuleInfo( 248, 3 ),
new _yyRuleInfo( 249, 1 ),
new _yyRuleInfo( 250, 0 ),
new _yyRuleInfo( 250, 4 ),
new _yyRuleInfo( 250, 2 ),
};

    //static void yy_accept(yyParser*);  /* Forward Declaration */

    /*
    ** Perform a reduce action and the shift that must immediately
    ** follow the reduce.
    */
    static void yy_reduce(
    yyParser yypParser,         /* The parser */
    int yyruleno                 /* Number of the rule by which to reduce */
    )
    {
      int yygoto;                     /* The next state */
      int yyact;                      /* The next action */
      YYMINORTYPE yygotominor;        /* The LHS of the rule reduced */
      yymsp yymsp; // yyStackEntry[] yymsp = new yyStackEntry[0];            /* The top of the parser's stack */
      int yysize;                     /* Amount to pop the stack */
      Parse pParse = yypParser.pParse; //sqlite3ParserARG_FETCH;

      yymsp = new yymsp( ref yypParser, yypParser.yyidx ); //      yymsp[0] = yypParser.yystack[yypParser.yyidx];
#if !NDEBUG
      if ( yyTraceFILE != null && yyruleno >= 0
      && yyruleno < yyRuleName.Length )
      { //(int)(yyRuleName.Length/sizeof(yyRuleName[0])) ){
        fprintf( yyTraceFILE, "%sReduce [%s].\n", yyTracePrompt,
        yyRuleName[yyruleno] );
      }
#endif // * NDEBUG */

      /* Silence complaints from purify about yygotominor being uninitialized
** in some cases when it is copied into the stack after the following
** switch.  yygotominor is uninitialized when a rule reduces that does
** not set the value of its left-hand side nonterminal.  Leaving the
** value of the nonterminal uninitialized is utterly harmless as long
** as the value is never used.  So really the only thing this code
** accomplishes is to quieten purify.
**
** 2007-01-16:  The wireshark project (www.wireshark.org) reports that
** without this code, their parser segfaults.  I'm not sure what there
** parser is doing to make this happen.  This is the second bug report
** from wireshark this week.  Clearly they are stressing Lemon in ways
** that it has not been previously stressed...  (SQLite ticket #2172)
*/
      yygotominor = new YYMINORTYPE(); //memset(yygotominor, 0, yygotominor).Length;
      switch ( yyruleno )
      {
        /* Beginning here are the reduction cases.  A typical example
        ** follows:
        **   case 0:
        **  //#line <lineno> <grammarfile>
        **     { ... }           // User supplied code
        **  //#line <lineno> <thisfile>
        **     break;
        */
        case 5: /* explain ::= */
          //#line 109 "parse.y"
          { sqlite3BeginParse( pParse, 0 ); }
          //#line 2047 "parse.c"
          break;
        case 6: /* explain ::= EXPLAIN */
          //#line 111 "parse.y"
          { sqlite3BeginParse( pParse, 1 ); }
          //#line 2052 "parse.c"
          break;
        case 7: /* explain ::= EXPLAIN QUERY PLAN */
          //#line 112 "parse.y"
          { sqlite3BeginParse( pParse, 2 ); }
          //#line 2057 "parse.c"
          break;
        case 8: /* cmdx ::= cmd */
          //#line 114 "parse.y"
          { sqlite3FinishCoding( pParse ); }
          //#line 2062 "parse.c"
          break;
        case 9: /* cmd ::= BEGIN transtype trans_opt */
          //#line 119 "parse.y"
          { sqlite3BeginTransaction( pParse, yymsp[-1].minor.yy194 ); }
          //#line 2067 "parse.c"
          break;
        case 13: /* transtype ::= */
          //#line 124 "parse.y"
          { yygotominor.yy194 = TK_DEFERRED; }
          //#line 2072 "parse.c"
          break;
        case 14: /* transtype ::= DEFERRED */
        case 15: /* transtype ::= IMMEDIATE */ //yytestcase(yyruleno==15);
        case 16: /* transtype ::= EXCLUSIVE */ //yytestcase(yyruleno==16);
        case 114: /* multiselect_op ::= UNION */ //yytestcase(yyruleno==114);
        case 116: /* multiselect_op ::= EXCEPT|INTERSECT */ //yytestcase(yyruleno==116);
          //#line 125 "parse.y"
          { yygotominor.yy194 = yymsp[0].major; }
          //#line 2081 "parse.c"
          break;
        case 17: /* cmd ::= COMMIT trans_opt */
        case 18: /* cmd ::= END trans_opt */ //yytestcase(yyruleno==18);
          //#line 128 "parse.y"
          { sqlite3CommitTransaction( pParse ); }
          //#line 2087 "parse.c"
          break;
        case 19: /* cmd ::= ROLLBACK trans_opt */
          //#line 130 "parse.y"
          { sqlite3RollbackTransaction( pParse ); }
          //#line 2092 "parse.c"
          break;
        case 22: /* cmd ::= SAVEPOINT nm */
          //#line 134 "parse.y"
          {
            sqlite3Savepoint( pParse, SAVEPOINT_BEGIN, yymsp[0].minor.yy0 );
          }
          //#line 2099 "parse.c"
          break;
        case 23: /* cmd ::= RELEASE savepoint_opt nm */
          //#line 137 "parse.y"
          {
            sqlite3Savepoint( pParse, SAVEPOINT_RELEASE, yymsp[0].minor.yy0 );
          }
          //#line 2106 "parse.c"
          break;
        case 24: /* cmd ::= ROLLBACK trans_opt TO savepoint_opt nm */
          //#line 140 "parse.y"
          {
            sqlite3Savepoint( pParse, SAVEPOINT_ROLLBACK, yymsp[0].minor.yy0 );
          }
          //#line 2113 "parse.c"
          break;
        case 26: /* create_table ::= createkw temp TABLE ifnotexists nm dbnm */
          //#line 147 "parse.y"
          {
            sqlite3StartTable( pParse, yymsp[-1].minor.yy0, yymsp[0].minor.yy0, yymsp[-4].minor.yy194, 0, 0, yymsp[-2].minor.yy194 );
          }
          //#line 2120 "parse.c"
          break;
        case 27: /* createkw ::= CREATE */
          //#line 150 "parse.y"
          {
            pParse.db.lookaside.bEnabled = 0;
            yygotominor.yy0 = yymsp[0].minor.yy0;
          }
          //#line 2128 "parse.c"
          break;
        case 28: /* ifnotexists ::= */
        case 31: /* temp ::= */ //yytestcase(yyruleno==31);
        case 70: /* autoinc ::= */ //yytestcase(yyruleno==70);
        case 84: /* init_deferred_pred_opt ::= */ //yytestcase(yyruleno==84);
        case 86: /* init_deferred_pred_opt ::= INITIALLY IMMEDIATE */ //yytestcase(yyruleno==86);
        case 97: /* defer_subclause_opt ::= */ //yytestcase(yyruleno==97);
        case 108: /* ifexists ::= */ //yytestcase(yyruleno==108);
        case 119: /* distinct ::= ALL */ //yytestcase(yyruleno==119);
        case 120: /* distinct ::= */ //yytestcase(yyruleno==120);
        case 222: /* between_op ::= BETWEEN */ //yytestcase(yyruleno==222);
        case 225: /* in_op ::= IN */ //yytestcase(yyruleno==225);
          //#line 155 "parse.y"
          { yygotominor.yy194 = 0; }
          //#line 2143 "parse.c"
          break;
        case 29: /* ifnotexists ::= IF NOT EXISTS */
        case 30: /* temp ::= TEMP */ //yytestcase(yyruleno==30);
        case 71: /* autoinc ::= AUTOINCR */ //yytestcase(yyruleno==71);
        case 85: /* init_deferred_pred_opt ::= INITIALLY DEFERRED */ //yytestcase(yyruleno==85);
        case 107: /* ifexists ::= IF EXISTS */ //yytestcase(yyruleno==107);
        case 118: /* distinct ::= DISTINCT */ //yytestcase(yyruleno==118);
        case 223: /* between_op ::= NOT BETWEEN */ //yytestcase(yyruleno==223);
        case 226: /* in_op ::= NOT IN */ //yytestcase(yyruleno==226);
          //#line 156 "parse.y"
          { yygotominor.yy194 = 1; }
          //#line 2155 "parse.c"
          break;
        case 32: /* create_table_args ::= LP columnlist conslist_opt RP */
          //#line 162 "parse.y"
          {
            sqlite3EndTable( pParse, yymsp[-1].minor.yy0, yymsp[0].minor.yy0, 0 );
          }
          //#line 2162 "parse.c"
          break;
        case 33: /* create_table_args ::= AS select */
          //#line 165 "parse.y"
          {
            sqlite3EndTable( pParse, 0, 0, yymsp[0].minor.yy243 );
            sqlite3SelectDelete( pParse.db, ref yymsp[0].minor.yy243 );
          }
          //#line 2170 "parse.c"
          break;
        case 36: /* column ::= columnid type carglist */
          //#line 177 "parse.y"
          {
            //yygotominor.yy0.z = yymsp[-2].minor.yy0.z;
            //yygotominor.yy0.n = (int)(pParse.sLastToken.z-yymsp[-2].minor.yy0.z) + pParse.sLastToken.n;
            yygotominor.yy0.n = (int)( yymsp[-2].minor.yy0.z.Length - pParse.sLastToken.z.Length ) + pParse.sLastToken.n;
            yygotominor.yy0.z = yymsp[-2].minor.yy0.z.Substring( 0, yygotominor.yy0.n );
          }
          //#line 2178 "parse.c"
          break;
        case 37: /* columnid ::= nm */
          //#line 181 "parse.y"
          {
            sqlite3AddColumn( pParse, yymsp[0].minor.yy0 );
            yygotominor.yy0 = yymsp[0].minor.yy0;
          }
          //#line 2186 "parse.c"
          break;
        case 38: /* id ::= ID */
        case 39: /* id ::= INDEXED */ //yytestcase(yyruleno==39);
        case 40: /* ids ::= ID|STRING */ //yytestcase(yyruleno==40);
        case 41: /* nm ::= id */ //yytestcase(yyruleno==41);
        case 42: /* nm ::= STRING */ //yytestcase(yyruleno==42);
        case 43: /* nm ::= JOIN_KW */ //yytestcase(yyruleno==43);
        case 46: /* typetoken ::= typename */ //yytestcase(yyruleno==46);
        case 49: /* typename ::= ids */ //yytestcase(yyruleno==49);
        case 126: /* as ::= AS nm */ //yytestcase(yyruleno==126);
        case 127: /* as ::= ids */ //yytestcase(yyruleno==127);
        case 137: /* dbnm ::= DOT nm */ //yytestcase(yyruleno==137);
        case 146: /* indexed_opt ::= INDEXED BY nm */ //yytestcase(yyruleno==146);
        case 251: /* collate ::= COLLATE ids */ //yytestcase(yyruleno==251);
        case 260: /* nmnum ::= plus_num */ //yytestcase(yyruleno==260);
        case 261: /* nmnum ::= nm */ //yytestcase(yyruleno==261);
        case 262: /* nmnum ::= ON */ //yytestcase(yyruleno==262);
        case 263: /* nmnum ::= DELETE */ //yytestcase(yyruleno==263);
        case 264: /* nmnum ::= DEFAULT */ //yytestcase(yyruleno==264);
        case 265: /* plus_num ::= plus_opt number */ //yytestcase(yyruleno==265);
        case 266: /* minus_num ::= MINUS number */ //yytestcase(yyruleno==266);
        case 267: /* number ::= INTEGER|FLOAT */ //yytestcase(yyruleno==267);
          //#line 191 "parse.y"
          { yygotominor.yy0 = yymsp[0].minor.yy0; }
          //#line 2211 "parse.c"
          break;
        case 45: /* type ::= typetoken */
          //#line 253 "parse.y"
          { sqlite3AddColumnType( pParse, yymsp[0].minor.yy0 ); }
          //#line 2216 "parse.c"
          break;
        case 47: /* typetoken ::= typename LP signed RP */
          //#line 255 "parse.y"
          {
            //yygotominor.yy0.z = yymsp[-3].minor.yy0.z;
            //yygotominor.yy0.n = (int)(yymsp[0].minor.yy0.z.Substring(yymsp[0].minor.yy0.n) - yymsp[-3].minor.yy0.z);
            yygotominor.yy0.n = yymsp[-3].minor.yy0.z.Length - yymsp[0].minor.yy0.z.Length + yymsp[0].minor.yy0.n;
            yygotominor.yy0.z = yymsp[-3].minor.yy0.z.Substring( 0, yygotominor.yy0.n );
          }
          //#line 2224 "parse.c"
          break;
        case 48: /* typetoken ::= typename LP signed COMMA signed RP */
          //#line 259 "parse.y"
          {
            //yygotominor.yy0.z = yymsp[-5].minor.yy0.z;
            //yygotominor.yy0.n = (int)(yymsp[0].minor.yy0.z.Substring(yymsp[0].minor.yy0.n) - yymsp[-5].minor.yy0.z);
            yygotominor.yy0.n = yymsp[-5].minor.yy0.z.Length - yymsp[0].minor.yy0.z.Length + 1;
            yygotominor.yy0.z = yymsp[-5].minor.yy0.z.Substring( 0, yygotominor.yy0.n );
          }
          //#line 2232 "parse.c"
          break;
        case 50: /* typename ::= typename ids */
          //#line 265 "parse.y"
          {
            //yygotominor.yy0.z=yymsp[-1].minor.yy0.z; yygotominor.yy0.n=yymsp[0].minor.yy0.n+(int)(yymsp[0].minor.yy0.z-yymsp[-1].minor.yy0.z);
            yygotominor.yy0.z = yymsp[-1].minor.yy0.z;
            yygotominor.yy0.n = yymsp[0].minor.yy0.n + (int)( yymsp[-1].minor.yy0.z.Length - yymsp[0].minor.yy0.z.Length );
          }
          //#line 2237 "parse.c"
          break;
        case 57: /* ccons ::= DEFAULT term */
        case 59: /* ccons ::= DEFAULT PLUS term */ //yytestcase(yyruleno==59);
          //#line 276 "parse.y"
          { sqlite3AddDefaultValue( pParse, yymsp[0].minor.yy190 ); }
          //#line 2243 "parse.c"
          break;
        case 58: /* ccons ::= DEFAULT LP expr RP */
          //#line 277 "parse.y"
          { sqlite3AddDefaultValue( pParse, yymsp[-1].minor.yy190 ); }
          //#line 2248 "parse.c"
          break;
        case 60: /* ccons ::= DEFAULT MINUS term */
          //#line 279 "parse.y"
          {
            ExprSpan v = new ExprSpan();
            v.pExpr = sqlite3PExpr( pParse, TK_UMINUS, yymsp[0].minor.yy190.pExpr, 0, 0 );
            v.zStart = yymsp[-1].minor.yy0.z.ToString();
            v.zEnd = yymsp[0].minor.yy190.zEnd;
            sqlite3AddDefaultValue( pParse, v );
          }
          //#line 2259 "parse.c"
          break;
        case 61: /* ccons ::= DEFAULT id */
          //#line 286 "parse.y"
          {
            ExprSpan v = new ExprSpan();
            spanExpr( v, pParse, TK_STRING, yymsp[0].minor.yy0 );
            sqlite3AddDefaultValue( pParse, v );
          }
          //#line 2268 "parse.c"
          break;
        case 63: /* ccons ::= NOT NULL onconf */
          //#line 296 "parse.y"
          { sqlite3AddNotNull( pParse, yymsp[0].minor.yy194 ); }
          //#line 2273 "parse.c"
          break;
        case 64: /* ccons ::= PRIMARY KEY sortorder onconf autoinc */
          //#line 298 "parse.y"
          { sqlite3AddPrimaryKey( pParse, 0, yymsp[-1].minor.yy194, yymsp[0].minor.yy194, yymsp[-2].minor.yy194 ); }
          //#line 2278 "parse.c"
          break;
        case 65: /* ccons ::= UNIQUE onconf */
          //#line 299 "parse.y"
          { sqlite3CreateIndex( pParse, 0, 0, 0, 0, yymsp[0].minor.yy194, 0, 0, 0, 0 ); }
          //#line 2283 "parse.c"
          break;
        case 66: /* ccons ::= CHECK LP expr RP */
          //#line 300 "parse.y"
          { sqlite3AddCheckConstraint( pParse, yymsp[-1].minor.yy190.pExpr ); }
          //#line 2288 "parse.c"
          break;
        case 67: /* ccons ::= REFERENCES nm idxlist_opt refargs */
          //#line 302 "parse.y"
          { sqlite3CreateForeignKey( pParse, 0, yymsp[-2].minor.yy0, yymsp[-1].minor.yy148, yymsp[0].minor.yy194 ); }
          //#line 2293 "parse.c"
          break;
        case 68: /* ccons ::= defer_subclause */
          //#line 303 "parse.y"
          { sqlite3DeferForeignKey( pParse, yymsp[0].minor.yy194 ); }
          //#line 2298 "parse.c"
          break;
        case 69: /* ccons ::= COLLATE ids */
          //#line 304 "parse.y"
          { sqlite3AddCollateType( pParse, yymsp[0].minor.yy0 ); }
          //#line 2303 "parse.c"
          break;
        case 72: /* refargs ::= */
          //#line 317 "parse.y"
          { yygotominor.yy194 = OE_Restrict * 0x010101; }
          //#line 2308 "parse.c"
          break;
        case 73: /* refargs ::= refargs refarg */
          //#line 318 "parse.y"
          { yygotominor.yy194 = ( yymsp[-1].minor.yy194 & ~yymsp[0].minor.yy497.mask ) | yymsp[0].minor.yy497.value; }
          //#line 2313 "parse.c"
          break;
        case 74: /* refarg ::= MATCH nm */
          //#line 320 "parse.y"
          { yygotominor.yy497.value = 0; yygotominor.yy497.mask = 0x000000; }
          //#line 2318 "parse.c"
          break;
        case 75: /* refarg ::= ON DELETE refact */
          //#line 321 "parse.y"
          { yygotominor.yy497.value = yymsp[0].minor.yy194; yygotominor.yy497.mask = 0x0000ff; }
          //#line 2323 "parse.c"
          break;
        case 76: /* refarg ::= ON UPDATE refact */
          //#line 322 "parse.y"
          { yygotominor.yy497.value = yymsp[0].minor.yy194 << 8; yygotominor.yy497.mask = 0x00ff00; }
          //#line 2328 "parse.c"
          break;
        case 77: /* refarg ::= ON INSERT refact */
          //#line 323 "parse.y"
          { yygotominor.yy497.value = yymsp[0].minor.yy194 << 16; yygotominor.yy497.mask = 0xff0000; }
          //#line 2333 "parse.c"
          break;
        case 78: /* refact ::= SET NULL */
          //#line 325 "parse.y"
          { yygotominor.yy194 = OE_SetNull; }
          //#line 2338 "parse.c"
          break;
        case 79: /* refact ::= SET DEFAULT */
          //#line 326 "parse.y"
          { yygotominor.yy194 = OE_SetDflt; }
          //#line 2343 "parse.c"
          break;
        case 80: /* refact ::= CASCADE */
          //#line 327 "parse.y"
          { yygotominor.yy194 = OE_Cascade; }
          //#line 2348 "parse.c"
          break;
        case 81: /* refact ::= RESTRICT */
          //#line 328 "parse.y"
          { yygotominor.yy194 = OE_Restrict; }
          //#line 2353 "parse.c"
          break;
        case 82: /* defer_subclause ::= NOT DEFERRABLE init_deferred_pred_opt */
        case 83: /* defer_subclause ::= DEFERRABLE init_deferred_pred_opt */ //yytestcase(yyruleno==83);
        case 98: /* defer_subclause_opt ::= defer_subclause */ //yytestcase(yyruleno==98);
        case 100: /* onconf ::= ON CONFLICT resolvetype */ //yytestcase(yyruleno==100);
        case 102: /* orconf ::= OR resolvetype */ //yytestcase(yyruleno==102);
        case 103: /* resolvetype ::= raisetype */ //yytestcase(yyruleno==103);
        case 175: /* insert_cmd ::= INSERT orconf */ //yytestcase(yyruleno==175);
          //#line 330 "parse.y"
          { yygotominor.yy194 = yymsp[0].minor.yy194; }
          //#line 2364 "parse.c"
          break;
        case 87: /* conslist_opt ::= */
          //#line 340 "parse.y"
          { yygotominor.yy0.n = 0; yygotominor.yy0.z = null; }
          //#line 2369 "parse.c"
          break;
        case 88: /* conslist_opt ::= COMMA conslist */
          //#line 341 "parse.y"
          { yygotominor.yy0 = yymsp[-1].minor.yy0; }
          //#line 2374 "parse.c"
          break;
        case 93: /* tcons ::= PRIMARY KEY LP idxlist autoinc RP onconf */
          //#line 347 "parse.y"
          { sqlite3AddPrimaryKey( pParse, yymsp[-3].minor.yy148, yymsp[0].minor.yy194, yymsp[-2].minor.yy194, 0 ); }
          //#line 2379 "parse.c"
          break;
        case 94: /* tcons ::= UNIQUE LP idxlist RP onconf */
          //#line 349 "parse.y"
          { sqlite3CreateIndex( pParse, 0, 0, 0, yymsp[-2].minor.yy148, yymsp[0].minor.yy194, 0, 0, 0, 0 ); }
          //#line 2384 "parse.c"
          break;
        case 95: /* tcons ::= CHECK LP expr RP onconf */
          //#line 351 "parse.y"
          { sqlite3AddCheckConstraint( pParse, yymsp[-2].minor.yy190.pExpr ); }
          //#line 2389 "parse.c"
          break;
        case 96: /* tcons ::= FOREIGN KEY LP idxlist RP REFERENCES nm idxlist_opt refargs defer_subclause_opt */
          //#line 353 "parse.y"
          {
            sqlite3CreateForeignKey( pParse, yymsp[-6].minor.yy148, yymsp[-3].minor.yy0, yymsp[-2].minor.yy148, yymsp[-1].minor.yy194 );
            sqlite3DeferForeignKey( pParse, yymsp[0].minor.yy194 );
          }
          //#line 2397 "parse.c"
          break;
        case 99: /* onconf ::= */
        case 101: /* orconf ::= */ //yytestcase(yyruleno==101);
          //#line 367 "parse.y"
          { yygotominor.yy194 = OE_Default; }
          //#line 2403 "parse.c"
          break;
        case 104: /* resolvetype ::= IGNORE */
          //#line 372 "parse.y"
          { yygotominor.yy194 = OE_Ignore; }
          //#line 2408 "parse.c"
          break;
        case 105: /* resolvetype ::= REPLACE */
        case 176: /* insert_cmd ::= REPLACE */ //yytestcase(yyruleno==176);
          //#line 373 "parse.y"
          { yygotominor.yy194 = OE_Replace; }
          //#line 2414 "parse.c"
          break;
        case 106: /* cmd ::= DROP TABLE ifexists fullname */
          //#line 377 "parse.y"
          {
            sqlite3DropTable( pParse, yymsp[0].minor.yy185, 0, yymsp[-1].minor.yy194 );
          }
          //#line 2421 "parse.c"
          break;
        case 109: /* cmd ::= createkw temp VIEW ifnotexists nm dbnm AS select */
          //#line 387 "parse.y"
          {
            sqlite3CreateView( pParse, yymsp[-7].minor.yy0, yymsp[-3].minor.yy0, yymsp[-2].minor.yy0, yymsp[0].minor.yy243, yymsp[-6].minor.yy194, yymsp[-4].minor.yy194 );
          }
          //#line 2428 "parse.c"
          break;
        case 110: /* cmd ::= DROP VIEW ifexists fullname */
          //#line 390 "parse.y"
          {
            sqlite3DropTable( pParse, yymsp[0].minor.yy185, 1, yymsp[-1].minor.yy194 );
          }
          //#line 2435 "parse.c"
          break;
        case 111: /* cmd ::= select */
          //#line 397 "parse.y"
          {
            SelectDest dest = new SelectDest( SRT_Output, '\0', 0, 0, 0 );
            sqlite3Select( pParse, yymsp[0].minor.yy243, ref dest );
            sqlite3SelectDelete( pParse.db, ref yymsp[0].minor.yy243 );
          }
          //#line 2444 "parse.c"
          break;
        case 112: /* select ::= oneselect */
          //#line 408 "parse.y"
          { yygotominor.yy243 = yymsp[0].minor.yy243; }
          //#line 2449 "parse.c"
          break;
        case 113: /* select ::= select multiselect_op oneselect */
          //#line 410 "parse.y"
          {
            if ( yymsp[0].minor.yy243 != null )
            {
              yymsp[0].minor.yy243.op = (u8)yymsp[-1].minor.yy194;
              yymsp[0].minor.yy243.pPrior = yymsp[-2].minor.yy243;
            }
            else
            {
              sqlite3SelectDelete( pParse.db, ref yymsp[-2].minor.yy243 );
            }
            yygotominor.yy243 = yymsp[0].minor.yy243;
          }
          //#line 2462 "parse.c"
          break;
        case 115: /* multiselect_op ::= UNION ALL */
          //#line 421 "parse.y"
          { yygotominor.yy194 = TK_ALL; }
          //#line 2467 "parse.c"
          break;
        case 117: /* oneselect ::= SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt */
          //#line 425 "parse.y"
          {
            yygotominor.yy243 = sqlite3SelectNew( pParse, yymsp[-6].minor.yy148, yymsp[-5].minor.yy185, yymsp[-4].minor.yy72, yymsp[-3].minor.yy148, yymsp[-2].minor.yy72, yymsp[-1].minor.yy148, yymsp[-7].minor.yy194, yymsp[0].minor.yy354.pLimit, yymsp[0].minor.yy354.pOffset );
          }
          //#line 2474 "parse.c"
          break;
        case 121: /* sclp ::= selcollist COMMA */
        case 247: /* idxlist_opt ::= LP idxlist RP */ //yytestcase(yyruleno==247);
          //#line 446 "parse.y"
          { yygotominor.yy148 = yymsp[-1].minor.yy148; }
          //#line 2480 "parse.c"
          break;
        case 122: /* sclp ::= */
        case 150: /* orderby_opt ::= */ //yytestcase(yyruleno==150);
        case 158: /* groupby_opt ::= */ //yytestcase(yyruleno==158);
        case 240: /* exprlist ::= */ //yytestcase(yyruleno==240);
        case 246: /* idxlist_opt ::= */ //yytestcase(yyruleno==246);
          //#line 447 "parse.y"
          { yygotominor.yy148 = null; }
          //#line 2489 "parse.c"
          break;
        case 123: /* selcollist ::= sclp expr as */
          //#line 448 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-2].minor.yy148, yymsp[-1].minor.yy190.pExpr );
            if ( yymsp[0].minor.yy0.n > 0 ) sqlite3ExprListSetName( pParse, yygotominor.yy148, yymsp[0].minor.yy0, 1 );
            sqlite3ExprListSetSpan( pParse, yygotominor.yy148, yymsp[-1].minor.yy190 );
          }
          //#line 2498 "parse.c"
          break;
        case 124: /* selcollist ::= sclp STAR */
          //#line 453 "parse.y"
          {
            Expr p = sqlite3Expr( pParse.db, TK_ALL, null );
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-1].minor.yy148, p );
          }
          //#line 2506 "parse.c"
          break;
        case 125: /* selcollist ::= sclp nm DOT STAR */
          //#line 457 "parse.y"
          {
            Expr pRight = sqlite3PExpr( pParse, TK_ALL, 0, 0, yymsp[0].minor.yy0 );
            Expr pLeft = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[-2].minor.yy0 );
            Expr pDot = sqlite3PExpr( pParse, TK_DOT, pLeft, pRight, 0 );
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-3].minor.yy148, pDot );
          }
          //#line 2516 "parse.c"
          break;
        case 128: /* as ::= */
          //#line 470 "parse.y"
          { yygotominor.yy0.n = 0; }
          //#line 2521 "parse.c"
          break;
        case 129: /* from ::= */
          //#line 482 "parse.y"
          { yygotominor.yy185 = new SrcList(); }//sqlite3DbMallocZero(pParse.db, sizeof(*yygotominor.yy185));}
          //#line 2526 "parse.c"
          break;
        case 130: /* from ::= FROM seltablist */
          //#line 483 "parse.y"
          {
            yygotominor.yy185 = yymsp[0].minor.yy185;
            sqlite3SrcListShiftJoinType( yygotominor.yy185 );
          }
          //#line 2534 "parse.c"
          break;
        case 131: /* stl_prefix ::= seltablist joinop */
          //#line 491 "parse.y"
          {
            yygotominor.yy185 = yymsp[-1].minor.yy185;
            if ( ALWAYS( yygotominor.yy185 != null && yygotominor.yy185.nSrc > 0 ) ) yygotominor.yy185.a[yygotominor.yy185.nSrc - 1].jointype = (u8)yymsp[0].minor.yy194;
          }
          //#line 2542 "parse.c"
          break;
        case 132: /* stl_prefix ::= */
          //#line 495 "parse.y"
          { yygotominor.yy185 = null; }
          //#line 2547 "parse.c"
          break;
        case 133: /* seltablist ::= stl_prefix nm dbnm as indexed_opt on_opt using_opt */
          //#line 496 "parse.y"
          {
            yygotominor.yy185 = sqlite3SrcListAppendFromTerm( pParse, yymsp[-6].minor.yy185, yymsp[-5].minor.yy0, yymsp[-4].minor.yy0, yymsp[-3].minor.yy0, 0, yymsp[-1].minor.yy72, yymsp[0].minor.yy254 );
            sqlite3SrcListIndexedBy( pParse, yygotominor.yy185, yymsp[-2].minor.yy0 );
          }
          //#line 2555 "parse.c"
          break;
        case 134: /* seltablist ::= stl_prefix LP select RP as on_opt using_opt */
          //#line 502 "parse.y"
          {
            yygotominor.yy185 = sqlite3SrcListAppendFromTerm( pParse, yymsp[-6].minor.yy185, 0, 0, yymsp[-2].minor.yy0, yymsp[-4].minor.yy243, yymsp[-1].minor.yy72, yymsp[0].minor.yy254 );
          }
          //#line 2562 "parse.c"
          break;
        case 135: /* seltablist ::= stl_prefix LP seltablist RP as on_opt using_opt */
          //#line 506 "parse.y"
          {
            if ( yymsp[-6].minor.yy185 == null )
            {
              sqlite3ExprDelete( pParse.db, ref yymsp[-1].minor.yy72 );
              sqlite3IdListDelete( pParse.db, ref yymsp[0].minor.yy254 );
              yygotominor.yy185 = yymsp[-4].minor.yy185;
            }
            else
            {
              Select pSubquery;
              sqlite3SrcListShiftJoinType( yymsp[-4].minor.yy185 );
              pSubquery = sqlite3SelectNew( pParse, 0, yymsp[-4].minor.yy185, 0, 0, 0, 0, 0, 0, 0 );
              yygotominor.yy185 = sqlite3SrcListAppendFromTerm( pParse, yymsp[-6].minor.yy185, 0, 0, yymsp[-2].minor.yy0, pSubquery, yymsp[-1].minor.yy72, yymsp[0].minor.yy254 );
            }
          }
          //#line 2578 "parse.c"
          break;
        case 136: /* dbnm ::= */
        case 145: /* indexed_opt ::= */ //yytestcase(yyruleno==145);
          //#line 533 "parse.y"
          { yygotominor.yy0.z = null; yygotominor.yy0.n = 0; }
          //#line 2584 "parse.c"
          break;
        case 138: /* fullname ::= nm dbnm */
          //#line 538 "parse.y"
          { yygotominor.yy185 = sqlite3SrcListAppend( pParse.db, 0, yymsp[-1].minor.yy0, yymsp[0].minor.yy0 ); }
          //#line 2589 "parse.c"
          break;
        case 139: /* joinop ::= COMMA|JOIN */
          //#line 542 "parse.y"
          { yygotominor.yy194 = JT_INNER; }
          //#line 2594 "parse.c"
          break;
        case 140: /* joinop ::= JOIN_KW JOIN */
          //#line 543 "parse.y"
          { yygotominor.yy194 = sqlite3JoinType( pParse, yymsp[-1].minor.yy0, 0, 0 ); }
          //#line 2599 "parse.c"
          break;
        case 141: /* joinop ::= JOIN_KW nm JOIN */
          //#line 544 "parse.y"
          { yygotominor.yy194 = sqlite3JoinType( pParse, yymsp[-2].minor.yy0, yymsp[-1].minor.yy0, 0 ); }
          //#line 2604 "parse.c"
          break;
        case 142: /* joinop ::= JOIN_KW nm nm JOIN */
          //#line 546 "parse.y"
          { yygotominor.yy194 = sqlite3JoinType( pParse, yymsp[-3].minor.yy0, yymsp[-2].minor.yy0, yymsp[-1].minor.yy0 ); }
          //#line 2609 "parse.c"
          break;
        case 143: /* on_opt ::= ON expr */
        case 154: /* sortitem ::= expr */ //yytestcase(yyruleno==154);
        case 161: /* having_opt ::= HAVING expr */ //yytestcase(yyruleno==161);
        case 168: /* where_opt ::= WHERE expr */ //yytestcase(yyruleno==168);
        case 235: /* case_else ::= ELSE expr */ //yytestcase(yyruleno==235);
        case 237: /* case_operand ::= expr */ //yytestcase(yyruleno==237);
          //#line 550 "parse.y"
          { yygotominor.yy72 = yymsp[0].minor.yy190.pExpr; }
          //#line 2619 "parse.c"
          break;
        case 144: /* on_opt ::= */
        case 160: /* having_opt ::= */ //yytestcase(yyruleno==160);
        case 167: /* where_opt ::= */ //yytestcase(yyruleno==167);
        case 236: /* case_else ::= */ //yytestcase(yyruleno==236);
        case 238: /* case_operand ::= */ //yytestcase(yyruleno==238);
          //#line 551 "parse.y"
          { yygotominor.yy72 = null; }
          //#line 2628 "parse.c"
          break;
        case 147: /* indexed_opt ::= NOT INDEXED */
          //#line 566 "parse.y"
          { yygotominor.yy0.z = null; yygotominor.yy0.n = 1; }
          //#line 2633 "parse.c"
          break;
        case 148: /* using_opt ::= USING LP inscollist RP */
        case 180: /* inscollist_opt ::= LP inscollist RP */ //yytestcase(yyruleno==180);
          //#line 570 "parse.y"
          { yygotominor.yy254 = yymsp[-1].minor.yy254; }
          //#line 2639 "parse.c"
          break;
        case 149: /* using_opt ::= */
        case 179: /* inscollist_opt ::= */ //yytestcase(yyruleno==179);
          //#line 571 "parse.y"
          { yygotominor.yy254 = null; }
          //#line 2645 "parse.c"
          break;
        case 151: /* orderby_opt ::= ORDER BY sortlist */
        case 159: /* groupby_opt ::= GROUP BY nexprlist */ //yytestcase(yyruleno==159);
        case 239: /* exprlist ::= nexprlist */ //yytestcase(yyruleno==239);
          //#line 582 "parse.y"
          { yygotominor.yy148 = yymsp[0].minor.yy148; }
          //#line 2652 "parse.c"
          break;
        case 152: /* sortlist ::= sortlist COMMA sortitem sortorder */
          //#line 583 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-3].minor.yy148, yymsp[-1].minor.yy72 );
            if ( yygotominor.yy148 != null ) yygotominor.yy148.a[yygotominor.yy148.nExpr - 1].sortOrder = (u8)yymsp[0].minor.yy194;
          }
          //#line 2660 "parse.c"
          break;
        case 153: /* sortlist ::= sortitem sortorder */
          //#line 587 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, 0, yymsp[-1].minor.yy72 );
            if ( yygotominor.yy148 != null && ALWAYS( yygotominor.yy148.a ) ) yygotominor.yy148.a[0].sortOrder = (u8)yymsp[0].minor.yy194;
          }
          //#line 2668 "parse.c"
          break;
        case 155: /* sortorder ::= ASC */
        case 157: /* sortorder ::= */ //yytestcase(yyruleno==157);
          //#line 595 "parse.y"
          { yygotominor.yy194 = SQLITE_SO_ASC; }
          //#line 2674 "parse.c"
          break;
        case 156: /* sortorder ::= DESC */
          //#line 596 "parse.y"
          { yygotominor.yy194 = SQLITE_SO_DESC; }
          //#line 2679 "parse.c"
          break;
        case 162: /* limit_opt ::= */
          //#line 622 "parse.y"
          { yygotominor.yy354.pLimit = null; yygotominor.yy354.pOffset = null; }
          //#line 2684 "parse.c"
          break;
        case 163: /* limit_opt ::= LIMIT expr */
          //#line 623 "parse.y"
          { yygotominor.yy354.pLimit = yymsp[0].minor.yy190.pExpr; yygotominor.yy354.pOffset = null; }
          //#line 2689 "parse.c"
          break;
        case 164: /* limit_opt ::= LIMIT expr OFFSET expr */
          //#line 625 "parse.y"
          { yygotominor.yy354.pLimit = yymsp[-2].minor.yy190.pExpr; yygotominor.yy354.pOffset = yymsp[0].minor.yy190.pExpr; }
          //#line 2694 "parse.c"
          break;
        case 165: /* limit_opt ::= LIMIT expr COMMA expr */
          //#line 627 "parse.y"
          { yygotominor.yy354.pOffset = yymsp[-2].minor.yy190.pExpr; yygotominor.yy354.pLimit = yymsp[0].minor.yy190.pExpr; }
          //#line 2699 "parse.c"
          break;
        case 166: /* cmd ::= DELETE FROM fullname indexed_opt where_opt */
          //#line 640 "parse.y"
          {
            sqlite3SrcListIndexedBy( pParse, yymsp[-2].minor.yy185, yymsp[-1].minor.yy0 );
            sqlite3DeleteFrom( pParse, yymsp[-2].minor.yy185, yymsp[0].minor.yy72 );
          }
          //#line 2707 "parse.c"
          break;
        case 169: /* cmd ::= UPDATE orconf fullname indexed_opt SET setlist where_opt */
          //#line 663 "parse.y"
          {
            sqlite3SrcListIndexedBy( pParse, yymsp[-4].minor.yy185, yymsp[-3].minor.yy0 );
            sqlite3ExprListCheckLength( pParse, yymsp[-1].minor.yy148, "set list" );
            sqlite3Update( pParse, yymsp[-4].minor.yy185, yymsp[-1].minor.yy148, yymsp[0].minor.yy72, yymsp[-5].minor.yy194 );
          }
          //#line 2716 "parse.c"
          break;
        case 170: /* setlist ::= setlist COMMA nm EQ expr */
          //#line 673 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-4].minor.yy148, yymsp[0].minor.yy190.pExpr );
            sqlite3ExprListSetName( pParse, yygotominor.yy148, yymsp[-2].minor.yy0, 1 );
          }
          //#line 2724 "parse.c"
          break;
        case 171: /* setlist ::= nm EQ expr */
          //#line 677 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, 0, yymsp[0].minor.yy190.pExpr );
            sqlite3ExprListSetName( pParse, yygotominor.yy148, yymsp[-2].minor.yy0, 1 );
          }
          //#line 2732 "parse.c"
          break;
        case 172: /* cmd ::= insert_cmd INTO fullname inscollist_opt VALUES LP itemlist RP */
          //#line 686 "parse.y"
          { sqlite3Insert( pParse, yymsp[-5].minor.yy185, yymsp[-1].minor.yy148, 0, yymsp[-4].minor.yy254, yymsp[-7].minor.yy194 ); }
          //#line 2737 "parse.c"
          break;
        case 173: /* cmd ::= insert_cmd INTO fullname inscollist_opt select */
          //#line 688 "parse.y"
          { sqlite3Insert( pParse, yymsp[-2].minor.yy185, 0, yymsp[0].minor.yy243, yymsp[-1].minor.yy254, yymsp[-4].minor.yy194 ); }
          //#line 2742 "parse.c"
          break;
        case 174: /* cmd ::= insert_cmd INTO fullname inscollist_opt DEFAULT VALUES */
          //#line 690 "parse.y"
          { sqlite3Insert( pParse, yymsp[-3].minor.yy185, 0, 0, yymsp[-2].minor.yy254, yymsp[-5].minor.yy194 ); }
          //#line 2747 "parse.c"
          break;
        case 177: /* itemlist ::= itemlist COMMA expr */
        case 241: /* nexprlist ::= nexprlist COMMA expr */ //yytestcase(yyruleno==241);
          //#line 701 "parse.y"
          { yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-2].minor.yy148, yymsp[0].minor.yy190.pExpr ); }
          //#line 2753 "parse.c"
          break;
        case 178: /* itemlist ::= expr */
        case 242: /* nexprlist ::= expr */ //yytestcase(yyruleno==242);
          //#line 703 "parse.y"
          { yygotominor.yy148 = sqlite3ExprListAppend( pParse, 0, yymsp[0].minor.yy190.pExpr ); }
          //#line 2759 "parse.c"
          break;
        case 181: /* inscollist ::= inscollist COMMA nm */
          //#line 713 "parse.y"
          { yygotominor.yy254 = sqlite3IdListAppend( pParse.db, yymsp[-2].minor.yy254, yymsp[0].minor.yy0 ); }
          //#line 2764 "parse.c"
          break;
        case 182: /* inscollist ::= nm */
          //#line 715 "parse.y"
          { yygotominor.yy254 = sqlite3IdListAppend( pParse.db, 0, yymsp[0].minor.yy0 ); }
          //#line 2769 "parse.c"
          break;
        case 183: /* expr ::= term */
        case 211: /* escape ::= ESCAPE expr */ //yytestcase(yyruleno==211);
          //#line 746 "parse.y"
          { yygotominor.yy190 = yymsp[0].minor.yy190; }
          //#line 2775 "parse.c"
          break;
        case 184: /* expr ::= LP expr RP */
          //#line 747 "parse.y"
          { yygotominor.yy190.pExpr = yymsp[-1].minor.yy190.pExpr; spanSet( yygotominor.yy190, yymsp[-2].minor.yy0, yymsp[0].minor.yy0 ); }
          //#line 2780 "parse.c"
          break;
        case 185: /* term ::= NULL */
        case 190: /* term ::= INTEGER|FLOAT|BLOB */ //yytestcase(yyruleno==190);
        case 191: /* term ::= STRING */ //yytestcase(yyruleno==191);
          //#line 748 "parse.y"
          { spanExpr( yygotominor.yy190, pParse, yymsp[0].major, yymsp[0].minor.yy0 ); }
          //#line 2787 "parse.c"
          break;
        case 186: /* expr ::= id */
        case 187: /* expr ::= JOIN_KW */ //yytestcase(yyruleno==187);
          //#line 749 "parse.y"
          { spanExpr( yygotominor.yy190, pParse, TK_ID, yymsp[0].minor.yy0 ); }
          //#line 2793 "parse.c"
          break;
        case 188: /* expr ::= nm DOT nm */
          //#line 751 "parse.y"
          {
            Expr temp1 = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[-2].minor.yy0 );
            Expr temp2 = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[0].minor.yy0 );
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_DOT, temp1, temp2, 0 );
            spanSet( yygotominor.yy190, yymsp[-2].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2803 "parse.c"
          break;
        case 189: /* expr ::= nm DOT nm DOT nm */
          //#line 757 "parse.y"
          {
            Expr temp1 = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[-4].minor.yy0 );
            Expr temp2 = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[-2].minor.yy0 );
            Expr temp3 = sqlite3PExpr( pParse, TK_ID, 0, 0, yymsp[0].minor.yy0 );
            Expr temp4 = sqlite3PExpr( pParse, TK_DOT, temp2, temp3, 0 );
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_DOT, temp1, temp4, 0 );
            spanSet( yygotominor.yy190, yymsp[-4].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2815 "parse.c"
          break;
        case 192: /* expr ::= REGISTER */
          //#line 767 "parse.y"
          {
            /* When doing a nested parse, one can include terms in an expression
            ** that look like this:   #1 #2 ...  These terms refer to registers
            ** in the virtual machine.  #N is the N-th register. */
            if ( pParse.nested == 0 )
            {
              sqlite3ErrorMsg( pParse, "near \"%T\": syntax error", yymsp[0].minor.yy0 );
              yygotominor.yy190.pExpr = null;
            }
            else
            {
              yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_REGISTER, 0, 0, yymsp[0].minor.yy0 );
              if ( yygotominor.yy190.pExpr != null ) sqlite3GetInt32( yymsp[0].minor.yy0.z.Substring( 1 ), ref yygotominor.yy190.pExpr.iTable );
            }
            spanSet( yygotominor.yy190, yymsp[0].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2832 "parse.c"
          break;
        case 193: /* expr ::= VARIABLE */
          //#line 780 "parse.y"
          {
            spanExpr( yygotominor.yy190, pParse, TK_VARIABLE, yymsp[0].minor.yy0 );
            sqlite3ExprAssignVarNumber( pParse, yygotominor.yy190.pExpr );
            spanSet( yygotominor.yy190, yymsp[0].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2841 "parse.c"
          break;
        case 194: /* expr ::= expr COLLATE ids */
          //#line 785 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3ExprSetColl( pParse, yymsp[-2].minor.yy190.pExpr, yymsp[0].minor.yy0 );
            yygotominor.yy190.zStart = yymsp[-2].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 2850 "parse.c"
          break;
        case 195: /* expr ::= CAST LP expr AS typetoken RP */
          //#line 791 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_CAST, yymsp[-3].minor.yy190.pExpr, 0, yymsp[-1].minor.yy0 );
            spanSet( yygotominor.yy190, yymsp[-5].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2858 "parse.c"
          break;
        case 196: /* expr ::= ID LP distinct exprlist RP */
          //#line 796 "parse.y"
          {
            if ( yymsp[-1].minor.yy148 != null && yymsp[-1].minor.yy148.nExpr > pParse.db.aLimit[SQLITE_LIMIT_FUNCTION_ARG] )
            {
              sqlite3ErrorMsg( pParse, "too many arguments on function %T", yymsp[-4].minor.yy0 );
            }
            yygotominor.yy190 = new ExprSpan();
            yygotominor.yy190.pExpr = sqlite3ExprFunction( pParse, yymsp[-1].minor.yy148, yymsp[-4].minor.yy0 );
            spanSet( yygotominor.yy190, yymsp[-4].minor.yy0, yymsp[0].minor.yy0 );
            if ( yymsp[-2].minor.yy194 != 0 && yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.flags |= EP_Distinct;
            }
          }
          //#line 2872 "parse.c"
          break;
        case 197: /* expr ::= ID LP STAR RP */
          //#line 806 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3ExprFunction( pParse, 0, yymsp[-3].minor.yy0 );
            spanSet( yygotominor.yy190, yymsp[-3].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2880 "parse.c"
          break;
        case 198: /* term ::= CTIME_KW */
          //#line 810 "parse.y"
          {
            /* The CURRENT_TIME, CURRENT_DATE, and CURRENT_TIMESTAMP values are
            ** treated as functions that return constants */
            yygotominor.yy190.pExpr = sqlite3ExprFunction( pParse, 0, yymsp[0].minor.yy0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.op = TK_CONST_FUNC;
            }
            spanSet( yygotominor.yy190, yymsp[0].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 2893 "parse.c"
          break;
        case 199: /* expr ::= expr AND expr */
        case 200: /* expr ::= expr OR expr */ //yytestcase(yyruleno==200);
        case 201: /* expr ::= expr LT|GT|GE|LE expr */ //yytestcase(yyruleno==201);
        case 202: /* expr ::= expr EQ|NE expr */ //yytestcase(yyruleno==202);
        case 203: /* expr ::= expr BITAND|BITOR|LSHIFT|RSHIFT expr */ //yytestcase(yyruleno==203);
        case 204: /* expr ::= expr PLUS|MINUS expr */ //yytestcase(yyruleno==204);
        case 205: /* expr ::= expr STAR|SLASH|REM expr */ //yytestcase(yyruleno==205);
        case 206: /* expr ::= expr CONCAT expr */ //yytestcase(yyruleno==206);
          //#line 837 "parse.y"
          { spanBinaryExpr( yygotominor.yy190, pParse, yymsp[-1].major, yymsp[-2].minor.yy190, yymsp[0].minor.yy190 ); }
          //#line 2905 "parse.c"
          break;
        case 207: /* likeop ::= LIKE_KW */
        case 209: /* likeop ::= MATCH */ //yytestcase(yyruleno==209);
          //#line 850 "parse.y"
          { yygotominor.yy392.eOperator = yymsp[0].minor.yy0; yygotominor.yy392.not = false; }
          //#line 2911 "parse.c"
          break;
        case 208: /* likeop ::= NOT LIKE_KW */
        case 210: /* likeop ::= NOT MATCH */ //yytestcase(yyruleno==210);
          //#line 851 "parse.y"
          { yygotominor.yy392.eOperator = yymsp[0].minor.yy0; yygotominor.yy392.not = true; }
          //#line 2917 "parse.c"
          break;
        case 212: /* escape ::= */
          //#line 857 "parse.y"
          { yygotominor.yy190 = new ExprSpan(); }// memset( yygotominor.yy190, 0, sizeof( yygotominor.yy190 ) ); }
          //#line 2922 "parse.c"
          break;
        case 213: /* expr ::= expr likeop expr escape */
          //#line 858 "parse.y"
          {
            ExprList pList;
            pList = sqlite3ExprListAppend( pParse, 0, yymsp[-1].minor.yy190.pExpr );
            pList = sqlite3ExprListAppend( pParse, pList, yymsp[-3].minor.yy190.pExpr );
            if ( yymsp[0].minor.yy190.pExpr != null )
            {
              pList = sqlite3ExprListAppend( pParse, pList, yymsp[0].minor.yy190.pExpr );
            }
            yygotominor.yy190.pExpr = sqlite3ExprFunction( pParse, pList, yymsp[-2].minor.yy392.eOperator );
            if ( yymsp[-2].minor.yy392.not ) yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_NOT, yygotominor.yy190.pExpr, 0, 0 );
            yygotominor.yy190.zStart = yymsp[-3].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[-1].minor.yy190.zEnd;
            if ( yygotominor.yy190.pExpr != null ) yygotominor.yy190.pExpr.flags |= EP_InfixFunc;
          }
          //#line 2939 "parse.c"
          break;
        case 214: /* expr ::= expr ISNULL|NOTNULL */
          //#line 888 "parse.y"
          { spanUnaryPostfix( yygotominor.yy190, pParse, yymsp[0].major, yymsp[-1].minor.yy190, yymsp[0].minor.yy0 ); }
          //#line 2944 "parse.c"
          break;
        case 215: /* expr ::= expr IS NULL */
          //#line 889 "parse.y"
          { spanUnaryPostfix( yygotominor.yy190, pParse, TK_ISNULL, yymsp[-2].minor.yy190, yymsp[0].minor.yy0 ); }
          //#line 2949 "parse.c"
          break;
        case 216: /* expr ::= expr NOT NULL */
          //#line 890 "parse.y"
          { spanUnaryPostfix( yygotominor.yy190, pParse, TK_NOTNULL, yymsp[-2].minor.yy190, yymsp[0].minor.yy0 ); }
          //#line 2954 "parse.c"
          break;
        case 217: /* expr ::= expr IS NOT NULL */
          //#line 892 "parse.y"
          { spanUnaryPostfix( yygotominor.yy190, pParse, TK_NOTNULL, yymsp[-3].minor.yy190, yymsp[0].minor.yy0 ); }
          //#line 2959 "parse.c"
          break;
        case 218: /* expr ::= NOT expr */
        case 219: /* expr ::= BITNOT expr */ //yytestcase(yyruleno==219);
          //#line 912 "parse.y"
          { spanUnaryPrefix( yygotominor.yy190, pParse, yymsp[-1].major, yymsp[0].minor.yy190, yymsp[-1].minor.yy0 ); }
          //#line 2965 "parse.c"
          break;
        case 220: /* expr ::= MINUS expr */
          //#line 915 "parse.y"
          { spanUnaryPrefix( yygotominor.yy190, pParse, TK_UMINUS, yymsp[0].minor.yy190, yymsp[-1].minor.yy0 ); }
          //#line 2970 "parse.c"
          break;
        case 221: /* expr ::= PLUS expr */
          //#line 917 "parse.y"
          { spanUnaryPrefix( yygotominor.yy190, pParse, TK_UPLUS, yymsp[0].minor.yy190, yymsp[-1].minor.yy0 ); }
          //#line 2975 "parse.c"
          break;
        case 224: /* expr ::= expr between_op expr AND expr */
          //#line 922 "parse.y"
          {
            ExprList pList = sqlite3ExprListAppend( pParse, 0, yymsp[-2].minor.yy190.pExpr );
            pList = sqlite3ExprListAppend( pParse, pList, yymsp[0].minor.yy190.pExpr );
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_BETWEEN, yymsp[-4].minor.yy190.pExpr, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pList = pList;
            }
            else
            {
              sqlite3ExprListDelete( pParse.db, ref pList );
            }
            if ( yymsp[-3].minor.yy194 != 0 ) yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_NOT, yygotominor.yy190.pExpr, 0, 0 );
            yygotominor.yy190.zStart = yymsp[-4].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[0].minor.yy190.zEnd;
          }
          //#line 2992 "parse.c"
          break;
        case 227: /* expr ::= expr in_op LP exprlist RP */
          //#line 939 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_IN, yymsp[-4].minor.yy190.pExpr, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pList = yymsp[-1].minor.yy148;
              sqlite3ExprSetHeight( pParse, yygotominor.yy190.pExpr );
            }
            else
            {
              sqlite3ExprListDelete( pParse.db, ref yymsp[-1].minor.yy148 );
            }
            if ( yymsp[-3].minor.yy194 != 0 ) yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_NOT, yygotominor.yy190.pExpr, 0, 0 );
            yygotominor.yy190.zStart = yymsp[-4].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3008 "parse.c"
          break;
        case 228: /* expr ::= LP select RP */
          //#line 951 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_SELECT, 0, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pSelect = yymsp[-1].minor.yy243;
              ExprSetProperty( yygotominor.yy190.pExpr, EP_xIsSelect );
              sqlite3ExprSetHeight( pParse, yygotominor.yy190.pExpr );
            }
            else
            {
              sqlite3SelectDelete( pParse.db, ref yymsp[-1].minor.yy243 );
            }
            yygotominor.yy190.zStart = yymsp[-2].minor.yy0.z.ToString();
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3024 "parse.c"
          break;
        case 229: /* expr ::= expr in_op LP select RP */
          //#line 963 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_IN, yymsp[-4].minor.yy190.pExpr, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pSelect = yymsp[-1].minor.yy243;
              ExprSetProperty( yygotominor.yy190.pExpr, EP_xIsSelect );
              sqlite3ExprSetHeight( pParse, yygotominor.yy190.pExpr );
            }
            else
            {
              sqlite3SelectDelete( pParse.db, ref yymsp[-1].minor.yy243 );
            }
            if ( yymsp[-3].minor.yy194 != 0 ) yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_NOT, yygotominor.yy190.pExpr, 0, 0 );
            yygotominor.yy190.zStart = yymsp[-4].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3041 "parse.c"
          break;
        case 230: /* expr ::= expr in_op nm dbnm */
          //#line 976 "parse.y"
          {
            SrcList pSrc = sqlite3SrcListAppend( pParse.db, 0, yymsp[-1].minor.yy0, yymsp[0].minor.yy0 );
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_IN, yymsp[-3].minor.yy190.pExpr, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pSelect = sqlite3SelectNew( pParse, 0, pSrc, 0, 0, 0, 0, 0, 0, 0 );
              ExprSetProperty( yygotominor.yy190.pExpr, EP_xIsSelect );
              sqlite3ExprSetHeight( pParse, yygotominor.yy190.pExpr );
            }
            else
            {
              sqlite3SrcListDelete( pParse.db, ref pSrc );
            }
            if ( yymsp[-2].minor.yy194 != 0 ) yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_NOT, yygotominor.yy190.pExpr, 0, 0 );
            yygotominor.yy190.zStart = yymsp[-3].minor.yy190.zStart;
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z != null ? yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n ) : yymsp[-1].minor.yy0.z.Substring( yymsp[-1].minor.yy0.n );
          }
          //#line 3059 "parse.c"
          break;
        case 231: /* expr ::= EXISTS LP select RP */
          //#line 990 "parse.y"
          {
            Expr p = yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_EXISTS, 0, 0, 0 );
            if ( p != null )
            {
              p.x.pSelect = yymsp[-1].minor.yy243;
              ExprSetProperty( p, EP_xIsSelect );
              sqlite3ExprSetHeight( pParse, p );
            }
            else
            {
              sqlite3SelectDelete( pParse.db, ref yymsp[-1].minor.yy243 );
            }
            yygotominor.yy190.zStart = yymsp[-3].minor.yy0.z.ToString();
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3075 "parse.c"
          break;
        case 232: /* expr ::= CASE case_operand case_exprlist case_else END */
          //#line 1005 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_CASE, yymsp[-3].minor.yy72, yymsp[-1].minor.yy72, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.x.pList = yymsp[-2].minor.yy148;
              sqlite3ExprSetHeight( pParse, yygotominor.yy190.pExpr );
            }
            else
            {
              sqlite3ExprListDelete( pParse.db, ref yymsp[-2].minor.yy148 );
            }
            yygotominor.yy190.zStart = yymsp[-4].minor.yy0.z.ToString();
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3090 "parse.c"
          break;
        case 233: /* case_exprlist ::= case_exprlist WHEN expr THEN expr */
          //#line 1018 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-4].minor.yy148, yymsp[-2].minor.yy190.pExpr );
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yygotominor.yy148, yymsp[0].minor.yy190.pExpr );
          }
          //#line 3098 "parse.c"
          break;
        case 234: /* case_exprlist ::= WHEN expr THEN expr */
          //#line 1022 "parse.y"
          {
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, 0, yymsp[-2].minor.yy190.pExpr );
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yygotominor.yy148, yymsp[0].minor.yy190.pExpr );
          }
          //#line 3106 "parse.c"
          break;
        case 243: /* cmd ::= createkw uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP */
          //#line 1051 "parse.y"
          {
            sqlite3CreateIndex( pParse, yymsp[-6].minor.yy0, yymsp[-5].minor.yy0,
            sqlite3SrcListAppend( pParse.db, 0, yymsp[-3].minor.yy0, 0 ), yymsp[-1].minor.yy148, yymsp[-9].minor.yy194,
            yymsp[-10].minor.yy0, yymsp[0].minor.yy0, SQLITE_SO_ASC, yymsp[-7].minor.yy194 );
          }
          //#line 3115 "parse.c"
          break;
        case 244: /* uniqueflag ::= UNIQUE */
        case 293: /* raisetype ::= ABORT */ //yytestcase(yyruleno==293);
          //#line 1058 "parse.y"
          { yygotominor.yy194 = OE_Abort; }
          //#line 3121 "parse.c"
          break;
        case 245: /* uniqueflag ::= */
          //#line 1059 "parse.y"
          { yygotominor.yy194 = OE_None; }
          //#line 3126 "parse.c"
          break;
        case 248: /* idxlist ::= idxlist COMMA nm collate sortorder */
          //#line 1068 "parse.y"
          {
            Expr p = null;
            if ( yymsp[-1].minor.yy0.n > 0 )
            {
              p = sqlite3Expr( pParse.db, TK_COLUMN, "" );
              sqlite3ExprSetColl( pParse, p, yymsp[-1].minor.yy0 );
            }
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, yymsp[-4].minor.yy148, p );
            sqlite3ExprListSetName( pParse, yygotominor.yy148, yymsp[-2].minor.yy0, 1 );
            sqlite3ExprListCheckLength( pParse, yygotominor.yy148, "index" );
            if ( yygotominor.yy148 != null ) yygotominor.yy148.a[yygotominor.yy148.nExpr - 1].sortOrder = (u8)yymsp[0].minor.yy194;
          }
          //#line 3141 "parse.c"
          break;
        case 249: /* idxlist ::= nm collate sortorder */
          //#line 1079 "parse.y"
          {
            Expr p = null;
            if ( yymsp[-1].minor.yy0.n > 0 )
            {
              p = sqlite3PExpr( pParse, TK_COLUMN, 0, 0, 0 );
              sqlite3ExprSetColl( pParse, p, yymsp[-1].minor.yy0 );
            }
            yygotominor.yy148 = sqlite3ExprListAppend( pParse, 0, p );
            sqlite3ExprListSetName( pParse, yygotominor.yy148, yymsp[-2].minor.yy0, 1 );
            sqlite3ExprListCheckLength( pParse, yygotominor.yy148, "index" );
            if ( yygotominor.yy148 != null ) yygotominor.yy148.a[yygotominor.yy148.nExpr - 1].sortOrder = (u8)yymsp[0].minor.yy194;
          }
          //#line 3156 "parse.c"
          break;
        case 250: /* collate ::= */
          //#line 1092 "parse.y"
          { yygotominor.yy0.z = null; yygotominor.yy0.n = 0; }
          //#line 3161 "parse.c"
          break;
        case 252: /* cmd ::= DROP INDEX ifexists fullname */
          //#line 1098 "parse.y"
          { sqlite3DropIndex( pParse, yymsp[0].minor.yy185, yymsp[-1].minor.yy194 ); }
          //#line 3166 "parse.c"
          break;
        case 253: /* cmd ::= VACUUM */
        case 254: /* cmd ::= VACUUM nm */ //yytestcase(yyruleno==254);
          //#line 1104 "parse.y"
          { sqlite3Vacuum( pParse ); }
          //#line 3172 "parse.c"
          break;
        case 255: /* cmd ::= PRAGMA nm dbnm */
          //#line 1112 "parse.y"
          { sqlite3Pragma( pParse, yymsp[-1].minor.yy0, yymsp[0].minor.yy0, 0, 0 ); }
          //#line 3177 "parse.c"
          break;
        case 256: /* cmd ::= PRAGMA nm dbnm EQ nmnum */
          //#line 1113 "parse.y"
          { sqlite3Pragma( pParse, yymsp[-3].minor.yy0, yymsp[-2].minor.yy0, yymsp[0].minor.yy0, 0 ); }
          //#line 3182 "parse.c"
          break;
        case 257: /* cmd ::= PRAGMA nm dbnm LP nmnum RP */
          //#line 1114 "parse.y"
          { sqlite3Pragma( pParse, yymsp[-4].minor.yy0, yymsp[-3].minor.yy0, yymsp[-1].minor.yy0, 0 ); }
          //#line 3187 "parse.c"
          break;
        case 258: /* cmd ::= PRAGMA nm dbnm EQ minus_num */
          //#line 1116 "parse.y"
          { sqlite3Pragma( pParse, yymsp[-3].minor.yy0, yymsp[-2].minor.yy0, yymsp[0].minor.yy0, 1 ); }
          //#line 3192 "parse.c"
          break;
        case 259: /* cmd ::= PRAGMA nm dbnm LP minus_num RP */
          //#line 1118 "parse.y"
          { sqlite3Pragma( pParse, yymsp[-4].minor.yy0, yymsp[-3].minor.yy0, yymsp[-1].minor.yy0, 1 ); }
          //#line 3197 "parse.c"
          break;
        case 270: /* cmd ::= createkw trigger_decl BEGIN trigger_cmd_list END */
          //#line 1136 "parse.y"
          {
            Token all = new Token();
            //all.z = yymsp[-3].minor.yy0.z;
            //all.n = (int)(yymsp[0].minor.yy0.z - yymsp[-3].minor.yy0.z) + yymsp[0].minor.yy0.n;
            all.n = (int)( yymsp[-3].minor.yy0.z.Length - yymsp[0].minor.yy0.z.Length ) + yymsp[0].minor.yy0.n;
            all.z = yymsp[-3].minor.yy0.z.Substring( 0, all.n );
            sqlite3FinishTrigger( pParse, yymsp[-1].minor.yy145, all );
          }
          //#line 3207 "parse.c"
          break;
        case 271: /* trigger_decl ::= temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event ON fullname foreach_clause when_clause */
          //#line 1145 "parse.y"
          {
            sqlite3BeginTrigger( pParse, yymsp[-7].minor.yy0, yymsp[-6].minor.yy0, yymsp[-5].minor.yy194, yymsp[-4].minor.yy332.a, yymsp[-4].minor.yy332.b, yymsp[-2].minor.yy185, yymsp[0].minor.yy72, yymsp[-10].minor.yy194, yymsp[-8].minor.yy194 );
            yygotominor.yy0 = ( yymsp[-6].minor.yy0.n == 0 ? yymsp[-7].minor.yy0 : yymsp[-6].minor.yy0 );
          }
          //#line 3215 "parse.c"
          break;
        case 272: /* trigger_time ::= BEFORE */
        case 275: /* trigger_time ::= */ //yytestcase(yyruleno==275);
          //#line 1151 "parse.y"
          { yygotominor.yy194 = TK_BEFORE; }
          //#line 3221 "parse.c"
          break;
        case 273: /* trigger_time ::= AFTER */
          //#line 1152 "parse.y"
          { yygotominor.yy194 = TK_AFTER; }
          //#line 3226 "parse.c"
          break;
        case 274: /* trigger_time ::= INSTEAD OF */
          //#line 1153 "parse.y"
          { yygotominor.yy194 = TK_INSTEAD; }
          //#line 3231 "parse.c"
          break;
        case 276: /* trigger_event ::= DELETE|INSERT */
        case 277: /* trigger_event ::= UPDATE */ //yytestcase(yyruleno==277);
          //#line 1158 "parse.y"
          { yygotominor.yy332.a = yymsp[0].major; yygotominor.yy332.b = null; }
          //#line 3237 "parse.c"
          break;
        case 278: /* trigger_event ::= UPDATE OF inscollist */
          //#line 1160 "parse.y"
          { yygotominor.yy332.a = TK_UPDATE; yygotominor.yy332.b = yymsp[0].minor.yy254; }
          //#line 3242 "parse.c"
          break;
        case 281: /* when_clause ::= */
        case 298: /* key_opt ::= */ //yytestcase(yyruleno==298);
          //#line 1167 "parse.y"
          { yygotominor.yy72 = null; }
          //#line 3248 "parse.c"
          break;
        case 282: /* when_clause ::= WHEN expr */
        case 299: /* key_opt ::= KEY expr */ //yytestcase(yyruleno==299);
          //#line 1168 "parse.y"
          { yygotominor.yy72 = yymsp[0].minor.yy190.pExpr; }
          //#line 3254 "parse.c"
          break;
        case 283: /* trigger_cmd_list ::= trigger_cmd_list trigger_cmd SEMI */
          //#line 1172 "parse.y"
          {
            Debug.Assert( yymsp[-2].minor.yy145 != null );
            yymsp[-2].minor.yy145.pLast.pNext = yymsp[-1].minor.yy145;
            yymsp[-2].minor.yy145.pLast = yymsp[-1].minor.yy145;
            yygotominor.yy145 = yymsp[-2].minor.yy145;
          }
          //#line 3264 "parse.c"
          break;
        case 284: /* trigger_cmd_list ::= trigger_cmd SEMI */
          //#line 1178 "parse.y"
          {
            Debug.Assert( yymsp[-1].minor.yy145 != null );
            yymsp[-1].minor.yy145.pLast = yymsp[-1].minor.yy145;
            yygotominor.yy145 = yymsp[-1].minor.yy145;
          }
          //#line 3273 "parse.c"
          break;
        case 285: /* trigger_cmd ::= UPDATE orconf nm SET setlist where_opt */
          //#line 1188 "parse.y"
          { yygotominor.yy145 = sqlite3TriggerUpdateStep( pParse.db, yymsp[-3].minor.yy0, yymsp[-1].minor.yy148, yymsp[0].minor.yy72, yymsp[-4].minor.yy194 ); }
          //#line 3278 "parse.c"
          break;
        case 286: /* trigger_cmd ::= insert_cmd INTO nm inscollist_opt VALUES LP itemlist RP */
          //#line 1193 "parse.y"
          { yygotominor.yy145 = sqlite3TriggerInsertStep( pParse.db, yymsp[-5].minor.yy0, yymsp[-4].minor.yy254, yymsp[-1].minor.yy148, 0, yymsp[-7].minor.yy194 ); }
          //#line 3283 "parse.c"
          break;
        case 287: /* trigger_cmd ::= insert_cmd INTO nm inscollist_opt select */
          //#line 1196 "parse.y"
          { yygotominor.yy145 = sqlite3TriggerInsertStep( pParse.db, yymsp[-2].minor.yy0, yymsp[-1].minor.yy254, 0, yymsp[0].minor.yy243, yymsp[-4].minor.yy194 ); }
          //#line 3288 "parse.c"
          break;
        case 288: /* trigger_cmd ::= DELETE FROM nm where_opt */
          //#line 1200 "parse.y"
          { yygotominor.yy145 = sqlite3TriggerDeleteStep( pParse.db, yymsp[-1].minor.yy0, yymsp[0].minor.yy72 ); }
          //#line 3293 "parse.c"
          break;
        case 289: /* trigger_cmd ::= select */
          //#line 1203 "parse.y"
          { yygotominor.yy145 = sqlite3TriggerSelectStep( pParse.db, yymsp[0].minor.yy243 ); }
          //#line 3298 "parse.c"
          break;
        case 290: /* expr ::= RAISE LP IGNORE RP */
          //#line 1206 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_RAISE, 0, 0, 0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.affinity = (char)OE_Ignore;
            }
            yygotominor.yy190.zStart = yymsp[-3].minor.yy0.z.ToString();
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3310 "parse.c"
          break;
        case 291: /* expr ::= RAISE LP raisetype COMMA nm RP */
          //#line 1214 "parse.y"
          {
            yygotominor.yy190.pExpr = sqlite3PExpr( pParse, TK_RAISE, 0, 0, yymsp[-1].minor.yy0 );
            if ( yygotominor.yy190.pExpr != null )
            {
              yygotominor.yy190.pExpr.affinity = (char)yymsp[-3].minor.yy194;
            }
            yygotominor.yy190.zStart = yymsp[-5].minor.yy0.z.ToString();
            yygotominor.yy190.zEnd = yymsp[0].minor.yy0.z.Substring( yymsp[0].minor.yy0.n );
          }
          //#line 3322 "parse.c"
          break;
        case 292: /* raisetype ::= ROLLBACK */
          //#line 1225 "parse.y"
          { yygotominor.yy194 = OE_Rollback; }
          //#line 3327 "parse.c"
          break;
        case 294: /* raisetype ::= FAIL */
          //#line 1227 "parse.y"
          { yygotominor.yy194 = OE_Fail; }
          //#line 3332 "parse.c"
          break;
        case 295: /* cmd ::= DROP TRIGGER ifexists fullname */
          //#line 1232 "parse.y"
          {
            sqlite3DropTrigger( pParse, yymsp[0].minor.yy185, yymsp[-1].minor.yy194 );
          }
          //#line 3339 "parse.c"
          break;
        case 296: /* cmd ::= ATTACH database_kw_opt expr AS expr key_opt */
          //#line 1239 "parse.y"
          {
            sqlite3Attach( pParse, yymsp[-3].minor.yy190.pExpr, yymsp[-1].minor.yy190.pExpr, yymsp[0].minor.yy72 );
          }
          //#line 3346 "parse.c"
          break;
        case 297: /* cmd ::= DETACH database_kw_opt expr */
          //#line 1242 "parse.y"
          {
            sqlite3Detach( pParse, yymsp[0].minor.yy190.pExpr );
          }
          //#line 3353 "parse.c"
          break;
        case 302: /* cmd ::= REINDEX */
          //#line 1257 "parse.y"
          { sqlite3Reindex( pParse, 0, 0 ); }
          //#line 3358 "parse.c"
          break;
        case 303: /* cmd ::= REINDEX nm dbnm */
          //#line 1258 "parse.y"
          { sqlite3Reindex( pParse, yymsp[-1].minor.yy0, yymsp[0].minor.yy0 ); }
          //#line 3363 "parse.c"
          break;
        case 304: /* cmd ::= ANALYZE */
          //#line 1263 "parse.y"
          { sqlite3Analyze( pParse, 0, 0 ); }
          //#line 3368 "parse.c"
          break;
        case 305: /* cmd ::= ANALYZE nm dbnm */
          //#line 1264 "parse.y"
          { sqlite3Analyze( pParse, yymsp[-1].minor.yy0, yymsp[0].minor.yy0 ); }
          //#line 3373 "parse.c"
          break;
        case 306: /* cmd ::= ALTER TABLE fullname RENAME TO nm */
          //#line 1269 "parse.y"
          {
            sqlite3AlterRenameTable( pParse, yymsp[-3].minor.yy185, yymsp[0].minor.yy0 );
          }
          //#line 3380 "parse.c"
          break;
        case 307: /* cmd ::= ALTER TABLE add_column_fullname ADD kwcolumn_opt column */
          //#line 1272 "parse.y"
          {
            sqlite3AlterFinishAddColumn( pParse, yymsp[0].minor.yy0 );
          }
          //#line 3387 "parse.c"
          break;
        case 308: /* add_column_fullname ::= fullname */
          //#line 1275 "parse.y"
          {
            pParse.db.lookaside.bEnabled = 0;
            sqlite3AlterBeginAddColumn( pParse, yymsp[0].minor.yy185 );
          }
          //#line 3395 "parse.c"
          break;
        case 311: /* cmd ::= create_vtab */
          //#line 1285 "parse.y"
          { sqlite3VtabFinishParse( pParse, 0 ); }
          //#line 3400 "parse.c"
          break;
        case 312: /* cmd ::= create_vtab LP vtabarglist RP */
          //#line 1286 "parse.y"
          { sqlite3VtabFinishParse( pParse, yymsp[0].minor.yy0 ); }
          //#line 3405 "parse.c"
          break;
        case 313: /* create_vtab ::= createkw VIRTUAL TABLE nm dbnm USING nm */
          //#line 1287 "parse.y"
          {
            sqlite3VtabBeginParse( pParse, yymsp[-3].minor.yy0, yymsp[-2].minor.yy0, yymsp[0].minor.yy0 );
          }
          //#line 3412 "parse.c"
          break;
        case 316: /* vtabarg ::= */
          //#line 1292 "parse.y"
          { sqlite3VtabArgInit( pParse ); }
          //#line 3417 "parse.c"
          break;
        case 318: /* vtabargtoken ::= ANY */
        case 319: /* vtabargtoken ::= lp anylist RP */ //yytestcase(yyruleno==319);
        case 320: /* lp ::= LP */ //yytestcase(yyruleno==320);
          //#line 1294 "parse.y"
          { sqlite3VtabArgExtend( pParse, yymsp[0].minor.yy0 ); }
          //#line 3424 "parse.c"
          break;
        default:
          /* (0) input ::= cmdlist */
          //yytestcase(yyruleno==0);
          /* (1) cmdlist ::= cmdlist ecmd */
          //yytestcase(yyruleno==1);
          /* (2) cmdlist ::= ecmd */
          //yytestcase(yyruleno==2);
          /* (3) ecmd ::= SEMI */
          //yytestcase(yyruleno==3);
          /* (4) ecmd ::= explain cmdx SEMI */
          //yytestcase(yyruleno==4);
          /* (10) trans_opt ::= */
          //yytestcase(yyruleno==10);
          /* (11) trans_opt ::= TRANSACTION */
          //yytestcase(yyruleno==11);
          /* (12) trans_opt ::= TRANSACTION nm */
          //yytestcase(yyruleno==12);
          /* (20) savepoint_opt ::= SAVEPOINT */
          //yytestcase(yyruleno==20);
          /* (21) savepoint_opt ::= */
          //yytestcase(yyruleno==21);
          /* (25) cmd ::= create_table create_table_args */
          //yytestcase(yyruleno==25);
          /* (34) columnlist ::= columnlist COMMA column */
          //yytestcase(yyruleno==34);
          /* (35) columnlist ::= column */
          //yytestcase(yyruleno==35);
          /* (44) type ::= */
          //yytestcase(yyruleno==44);
          /* (51) signed ::= plus_num */
          //yytestcase(yyruleno==51);
          /* (52) signed ::= minus_num */
          //yytestcase(yyruleno==52);
          /* (53) carglist ::= carglist carg */
          //yytestcase(yyruleno==53);
          /* (54) carglist ::= */
          //yytestcase(yyruleno==54);
          /* (55) carg ::= CONSTRAINT nm ccons */
          //yytestcase(yyruleno==55);
          /* (56) carg ::= ccons */
          //yytestcase(yyruleno==56);
          /* (62) ccons ::= NULL onconf */
          //yytestcase(yyruleno==62);
          /* (89) conslist ::= conslist COMMA tcons */
          //yytestcase(yyruleno==89);
          /* (90) conslist ::= conslist tcons */
          //yytestcase(yyruleno==90);
          /* (91) conslist ::= tcons */
          //yytestcase(yyruleno==91);
          /* (92) tcons ::= CONSTRAINT nm */
          //yytestcase(yyruleno==92);
          /* (268) plus_opt ::= PLUS */
          //yytestcase(yyruleno==268);
          /* (269) plus_opt ::= */
          //yytestcase(yyruleno==269);
          /* (279) foreach_clause ::= */
          //yytestcase(yyruleno==279);
          /* (280) foreach_clause ::= FOR EACH ROW */
          //yytestcase(yyruleno==280);
          /* (300) database_kw_opt ::= DATABASE */
          //yytestcase(yyruleno==300);
          /* (301) database_kw_opt ::= */
          //yytestcase(yyruleno==301);
          /* (309) kwcolumn_opt ::= */
          //yytestcase(yyruleno==309);
          /* (310) kwcolumn_opt ::= COLUMNKW */
          //yytestcase(yyruleno==310);
          /* (314) vtabarglist ::= vtabarg */
          //yytestcase(yyruleno==314);
          /* (315) vtabarglist ::= vtabarglist COMMA vtabarg */
          //yytestcase(yyruleno==315);
          /* (317) vtabarg ::= vtabarg vtabargtoken */
          //yytestcase(yyruleno==317);
          /* (321) anylist ::= */
          //yytestcase(yyruleno==321);
          /* (322) anylist ::= anylist LP anylist RP */
          //yytestcase(yyruleno==322);
          /* (323) anylist ::= anylist ANY */
          //yytestcase(yyruleno==323);
          break;
      };
      yygoto = yyRuleInfo[yyruleno].lhs;
      yysize = yyRuleInfo[yyruleno].nrhs;
      yypParser.yyidx -= yysize;
      yyact = yy_find_reduce_action( yymsp[-yysize].stateno, (YYCODETYPE)yygoto );
      if ( yyact < YYNSTATE )
      {
#if NDEBUG
/* If we are not debugging and the reduce action popped at least
** one element off the stack, then we can push the new element back
** onto the stack here, and skip the stack overflow test in yy_shift().
** That gives a significant speed improvement. */
if( yysize!=0 ){
yypParser.yyidx++;
yymsp._yyidx -= yysize - 1;
yymsp[0].stateno = (YYACTIONTYPE)yyact;
yymsp[0].major = (YYCODETYPE)yygoto;
yymsp[0].minor = yygotominor;
}else
#endif
        {
          yy_shift( yypParser, yyact, yygoto, yygotominor );
        }
      }
      else
      {
        Debug.Assert( yyact == YYNSTATE + YYNRULE + 1 );
        yy_accept( yypParser );
      }
    }

    /*
    ** The following code executes when the parse fails
    */
#if !YYNOERRORRECOVERY
    static void yy_parse_failed(
    yyParser yypParser           /* The parser */
    )
    {
      Parse pParse = yypParser.pParse; //       sqlite3ParserARG_FETCH;
#if !NDEBUG
      if ( yyTraceFILE != null )
      {
        Debugger.Break(); // TODO --        fprintf(yyTraceFILE, "%sFail!\n", yyTracePrompt);
      }
#endif
      while ( yypParser.yyidx >= 0 ) yy_pop_parser_stack( yypParser );
      /* Here code is inserted which will be executed whenever the
      ** parser fails */
      yypParser.pParse = pParse;//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
    }
#endif //* YYNOERRORRECOVERY */

    /*
** The following code executes when a syntax error first occurs.
*/
    static void yy_syntax_error(
    yyParser yypParser,           /* The parser */
    int yymajor,                   /* The major type of the error token */
    YYMINORTYPE yyminor            /* The minor type of the error token */
    )
    {
      Parse pParse = yypParser.pParse; //       sqlite3ParserARG_FETCH;
      //#define TOKEN (yyminor.yy0)
      //#line 34 "parse.y"

      UNUSED_PARAMETER( yymajor );  /* Silence some compiler warnings */
      Debug.Assert( yyminor.yy0.z.Length > 0 ); //TOKEN.z[0]);  /* The tokenizer always gives us a token */
      sqlite3ErrorMsg( pParse, "near \"%T\": syntax error", yyminor.yy0 );//&TOKEN);
      pParse.parseError = 1;
      //#line 3531 "parse.c"
      yypParser.pParse = pParse; // sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
    }

    /*
    ** The following is executed when the parser accepts
    */
    static void yy_accept(
    yyParser yypParser           /* The parser */
    )
    {
      Parse pParse = yypParser.pParse; //       sqlite3ParserARG_FETCH;
#if !NDEBUG
      if ( yyTraceFILE != null )
      {
        fprintf( yyTraceFILE, "%sAccept!\n", yyTracePrompt );
      }
#endif
      while ( yypParser.yyidx >= 0 ) yy_pop_parser_stack( yypParser );
      /* Here code is inserted which will be executed whenever the
      ** parser accepts */
      yypParser.pParse = pParse;//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
    }

    /* The main parser program.
    ** The first argument is a pointer to a structure obtained from
    ** "sqlite3ParserAlloc" which describes the current state of the parser.
    ** The second argument is the major token number.  The third is
    ** the minor token.  The fourth optional argument is whatever the
    ** user wants (and specified in the grammar) and is available for
    ** use by the action routines.
    **
    ** Inputs:
    ** <ul>
    ** <li> A pointer to the parser (an opaque structure.)
    ** <li> The major token number.
    ** <li> The minor token number.
    ** <li> An option argument of a grammar-specified type.
    ** </ul>
    **
    ** Outputs:
    ** None.
    */
    static void sqlite3Parser(
    yyParser yyp,                   /* The parser */
    int yymajor,                     /* The major token code number */
    sqlite3ParserTOKENTYPE yyminor  /* The value for the token */
    , Parse pParse //sqlite3ParserARG_PDECL           /* Optional %extra_argument parameter */
    )
    {
      YYMINORTYPE yyminorunion = new YYMINORTYPE();
      int yyact;            /* The parser action. */
      bool yyendofinput;     /* True if we are at the end of input */
#if YYERRORSYMBOL
int yyerrorhit = 0;   /* True if yymajor has invoked an error */
#endif
      yyParser yypParser;  /* The parser */

      /* (re)initialize the parser, if necessary */
      yypParser = yyp;
      if ( yypParser.yyidx < 0 )
      {
#if YYSTACKDEPTH//<=0
if( yypParser.yystksz <=0 ){
memset(yyminorunion, 0, yyminorunion).Length;
yyStackOverflow(yypParser, yyminorunion);
return;
}
#endif
        yypParser.yyidx = 0;
        yypParser.yyerrcnt = -1;
        yypParser.yystack[0] = new yyStackEntry();
        yypParser.yystack[0].stateno = 0;
        yypParser.yystack[0].major = 0;
      }
      yyminorunion.yy0 = yyminor.Copy();
      yyendofinput = ( yymajor == 0 );
      yypParser.pParse = pParse;//      sqlite3ParserARG_STORE;

#if !NDEBUG
      if ( yyTraceFILE != null )
      {
        fprintf( yyTraceFILE, "%sInput %s\n", yyTracePrompt, yyTokenName[yymajor] );
      }
#endif

      do
      {
        yyact = yy_find_shift_action( yypParser, (YYCODETYPE)yymajor );
        if ( yyact < YYNSTATE )
        {
          Debug.Assert( !yyendofinput );  /* Impossible to shift the $ token */
          yy_shift( yypParser, yyact, yymajor, yyminorunion );
          yypParser.yyerrcnt--;
          yymajor = YYNOCODE;
        }
        else if ( yyact < YYNSTATE + YYNRULE )
        {
          yy_reduce( yypParser, yyact - YYNSTATE );
        }
        else
        {
          Debug.Assert( yyact == YY_ERROR_ACTION );
#if YYERRORSYMBOL
int yymx;
#endif
#if !NDEBUG
          if ( yyTraceFILE != null )
          {
            Debugger.Break(); // TODO --            fprintf(yyTraceFILE, "%sSyntax Error!\n", yyTracePrompt);
          }
#endif
#if YYERRORSYMBOL
/* A syntax error has occurred.
** The response to an error depends upon whether or not the
** grammar defines an error token "ERROR".
**
** This is what we do if the grammar does define ERROR:
**
**  * Call the %syntax_error function.
**
**  * Begin popping the stack until we enter a state where
**    it is legal to shift the error symbol, then shift
**    the error symbol.
**
**  * Set the error count to three.
**
**  * Begin accepting and shifting new tokens.  No new error
**    processing will occur until three tokens have been
**    shifted successfully.
**
*/
if( yypParser.yyerrcnt<0 ){
yy_syntax_error(yypParser,yymajor,yyminorunion);
}
yymx = yypParser.yystack[yypParser.yyidx].major;
if( yymx==YYERRORSYMBOL || yyerrorhit ){
#if !NDEBUG
if( yyTraceFILE ){
Debug.Assert(false); // TODO --                      fprintf(yyTraceFILE,"%sDiscard input token %s\n",
yyTracePrompt,yyTokenName[yymajor]);
}
#endif
yy_destructor(yypParser,(YYCODETYPE)yymajor,yyminorunion);
yymajor = YYNOCODE;
}else{
while(
yypParser.yyidx >= 0 &&
yymx != YYERRORSYMBOL &&
(yyact = yy_find_reduce_action(
yypParser.yystack[yypParser.yyidx].stateno,
YYERRORSYMBOL)) >= YYNSTATE
){
yy_pop_parser_stack(yypParser);
}
if( yypParser.yyidx < 0 || yymajor==0 ){
yy_destructor(yypParser, (YYCODETYPE)yymajor,yyminorunion);
yy_parse_failed(yypParser);
yymajor = YYNOCODE;
}else if( yymx!=YYERRORSYMBOL ){
YYMINORTYPE u2;
u2.YYERRSYMDT = 0;
yy_shift(yypParser,yyact,YYERRORSYMBOL,u2);
}
}
yypParser.yyerrcnt = 3;
yyerrorhit = 1;
#elif (YYNOERRORRECOVERY)
/* If the YYNOERRORRECOVERY macro is defined, then do not attempt to
** do any kind of error recovery.  Instead, simply invoke the syntax
** error routine and continue going as if nothing had happened.
**
** Applications can set this macro (for example inside %include) if
** they intend to abandon the parse upon the first syntax error seen.
*/
yy_syntax_error(yypParser,yymajor,yyminorunion);
yy_destructor(yypParser,(YYCODETYPE)yymajor,yyminorunion);
yymajor = YYNOCODE;
#else  // * YYERRORSYMBOL is not defined */
          /* This is what we do if the grammar does not define ERROR:
**
**  * Report an error message, and throw away the input token.
**
**  * If the input token is $, then fail the parse.
**
** As before, subsequent error messages are suppressed until
** three input tokens have been successfully shifted.
*/
          if ( yypParser.yyerrcnt <= 0 )
          {
            yy_syntax_error( yypParser, yymajor, yyminorunion );
          }
          yypParser.yyerrcnt = 3;
          yy_destructor( yypParser, (YYCODETYPE)yymajor, yyminorunion );
          if ( yyendofinput )
          {
            yy_parse_failed( yypParser );
          }
          yymajor = YYNOCODE;
#endif
        }
      } while ( yymajor != YYNOCODE && yypParser.yyidx >= 0 );
      return;
    }
    public class yymsp
    {
      public yyParser _yyParser;
      public int _yyidx;
      // CONSTRUCTOR
      public yymsp( ref yyParser pointer_to_yyParser, int yyidx ) //' Parser and Stack Index
      {
        this._yyParser = pointer_to_yyParser;
        this._yyidx = yyidx;
      }
      // Default Value
      public yyStackEntry this[int offset]
      {
        get { return _yyParser.yystack[_yyidx + offset]; }
      }
    }
  }
}
