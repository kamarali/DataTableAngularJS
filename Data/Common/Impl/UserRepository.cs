using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core.DI;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        /// <summary>
        /// MemCache Manager Instance.
        /// </summary>
        private readonly ICacheManager _cacheManager;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository()
        {
            _cacheManager = Ioc.Resolve<ICacheManager>();
        }


        

        /// <summary>
        /// Adds the user permission.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userPermissions">The user permissions.</param>
        public void SaveUserPermissions(int userId, IList<int> userPermissions)
        {
            var key = GetCacheKey(userId);
            _cacheManager.Add(key, userPermissions);
        }

        /// <summary>
        /// Gets the user permission.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<int> GetUserPermissions(int userId)
        {
            var permissions = (IList<int>)_cacheManager.Get(GetCacheKey(userId));
            return permissions;
        }

        /// <summary>
        /// Removes the user permission.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void RemoveUserPermission(int userId)
        {
            var key = GetCacheKey(userId);
            _cacheManager.Remove(key);
        }

        /// <summary>
        /// Gets users given the member Id.
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public List<User> GetUsersByMemberId(int memberId)
        {
            var parameters = new ObjectParameter[1];

            parameters[0] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };

            var users = ExecuteStoredFunction<User>("GetUsersByMemberId", parameters);

            return users.ToList();
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <param name="userId">The MiscCode id.</param>
        /// <returns></returns>
        private static string GetCacheKey(int userId)
        {
            return ("User" + "_" + userId);
        }

        /// <summary>
        /// This will load list of User objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<User> LoadEntities(ObjectSet<User> objectSet, LoadStrategyResult loadStrategyResult, Action<User> link, string entity)
        {
            if (link == null)
                link = new Action<User>(c => { });

            var users = new List<User>();

            var commonMaterializers = new CommonMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {

                // first result set includes the category
                foreach (var c in
                    commonMaterializers.UserMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    users.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return users;
        }


    }

   
  
}
