using HMS.Domain.Entities;
namespace HMS.Domain.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// <para>Execute any Stored Procedure where a return data set it not expected.</para>
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        void ExecuteSp(string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute any Stored Procedure where a return data set it not expected.</para>
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        Task ExecuteSpAsync(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute any Stored Procedure where a single item is expected as a return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>A single instance of type T.</returns>
        T ExecuteSpSingle<T>(string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute any Stored Procedure where a single item is expected as a return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>A single instance of type T.</returns>
        Task<T> ExecuteSpSingleAsync<T>(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute any Stored Procedure where a return value is expected as a return.</para>
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>long value</returns>
        long ExecuteSpReturnValue(string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute any Stored Procedure where a return value is expected as a return.</para>
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>long value</returns>
        Task<long> ExecuteSpReturnValueAsync(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute a Store Procedure when a List of T is expected in return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>An IEnumerable of type T.</returns>
        IEnumerable<T> ExecuteSpList<T>(string storedProcedureName, List<ParametersCollection> parameters = null);

        /// <summary>
        /// <para>Execute a Store Procedure when a List of T is expected in return.</para>
        /// </summary>
        /// <typeparam name="T">The type that matches the database table.</typeparam>
        /// <param name="token">Cancellation Token</param>
        /// <param name="storedProcedureName">Name of the stored procedure to be executed.</param>
        /// <param name="parameters">Optional set of parameters that matches the query.</param>
        /// <returns>An IEnumerable of type T.</returns>
        Task<IEnumerable<T>> ExecuteSpListAsync<T>(CancellationToken token, string storedProcedureName, List<ParametersCollection> parameters = null);
    }
}
