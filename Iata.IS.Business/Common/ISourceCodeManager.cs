using System.Collections.Generic;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface ISourceCodeManager
    {
        /// <summary>
        /// Adds the source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns></returns>
        SourceCode AddSourceCode(SourceCode sourceCode);

        /// <summary>
        /// Updates the source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns></returns>
        SourceCode UpdateSourceCode(SourceCode sourceCode);

        /// <summary>
        /// Deletes the source code.
        /// </summary>
        /// <param name="sourceCodeId">The source code id.</param>
        /// <returns></returns>
        bool DeleteSourceCode(int sourceCodeId);

        /// <summary>
        /// Gets the source code details.
        /// </summary>
        /// <param name="sourceCodeId">The source code id.</param>
        /// <returns></returns>
        SourceCode GetSourceCodeDetails(int sourceCodeId);

        /// <summary>
        /// Gets all source code list.
        /// </summary>
        /// <returns></returns>
        List<SourceCode> GetAllSourceCodeList();

        /// <summary>
        /// Gets the source code list.
        /// </summary>
        /// <param name="SourceCodeIdentifier">The source code identifier.</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="UtilizationType">Type of the utilization.</param>
        /// <returns></returns>
        List<SourceCode> GetSourceCodeList(int SourceCodeIdentifier, int transactionTypeId, string UtilizationType);
    }
}
