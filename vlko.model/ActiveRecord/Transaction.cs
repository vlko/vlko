using Castle.ActiveRecord;
using GenericRepository;

namespace vlko.model.ActiveRecord
{
    /// <summary>
    /// Active record transaction implementation.
    /// </summary>
    public class Transaction : TransactionScope, ITransaction
    {

        public void Commit()
        {
            VoteCommit();
        }

        public void Rollback()
        {
            VoteRollBack();
        }

    }
}


