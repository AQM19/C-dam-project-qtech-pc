using System.Data;
using System.Data.SqlClient;

namespace AccesData.DataBase
{
    public class ClsDataBase
    {
        #region Private Variables

        private SqlConnection _objSqlConnection;
        private SqlDataAdapter _objSqlDataAdapter;
        private SqlCommand _objSqlCommand;
        private DataSet _dsResults; // Resultado del comando
        private DataTable _dtParams; // Parametros de los procedimientos almacenados
        private string _tableName, _spName, _errorMessageDB, _scalarValue, _dbName;
        private bool _scalar;

        #endregion

        #region Public Variables

        public SqlConnection ObjSqlConnection { get; set; }
        public SqlDataAdapter ObjSqlDataAdapter { get; set; }
        public SqlCommand ObjSqlCommand { get; set; }
        public DataSet DtResults { get; set; }
        public DataTable DtParams { get; set; }
        public string TableName { get; set; }
        public string SpName { get; set; }
        public string ErrorMessageDB { get; set; }
        public string ScalarValue { get; set; }
        public string DbName { get; set; }
        public bool Scalar { get; set; }

        #endregion

        #region Constructors

        public ClsDataBase()
        {
            DtParams = new DataTable("SpParams");
                DtParams.Columns.Add("Name");
                DtParams.Columns.Add("DateType");
                DtParams.Columns.Add("Value");

            DbName = string.Empty;
        }

        #endregion

        #region Private Methods

        private void CreateConnectonDatabase(ref ClsDataBase ObjDataBase) { }
        private void ValidateConnectonDatabase(ref ClsDataBase ObjDataBase) { }
        private void PrepareConnectionDatabase(ref ClsDataBase ObjDataBase) { }
        private void AddParams(ref ClsDataBase ObjDataBase) { }
        private void ExecDataAdapter(ref ClsDataBase ObjDataBase) { }
        private void ExecCommand(ref ClsDataBase ObjDataBase) { }

        #endregion

        #region Public Methods
        #endregion
    }
}
