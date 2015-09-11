using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModule
{
    public interface IDataPreparator
    {
        void FetchData(DataStore.MainInterface dataStore);

        void Calculate();
    }
}
