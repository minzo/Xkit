using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Externalkit.Perforce
{
    /// <summary>
    /// ファイルに行ったアクション
    /// </summary>
    public enum P4FileAction
    {
        None,
        Add,
        Edit,
        Delete,
        Branch,
        MoveAdd,
        MoveDelete,
        Integrate,
        Import,
        Purge,
        Archive
    }
}
