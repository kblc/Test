using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleSimpleCount
{
    public class Implementation : AnalysisModule.IDataPreparator
    {
        public void FetchData(DataStore.MainInterface dataStore)
        {
            data = new List<InternalData>();

            var tempDic = dataStore.TimestampComponents.ToDictionary( f=>f.EntityId);

            data.AddRange(
                dataStore.HeightComponents
                .Where(hc => !hc.IsMissing && !tempDic[hc.EntityId].IsMissing)
                .Select(hc => new InternalData { Height = hc.Value }));
        }

        List<InternalData> data;

        public void Calculate()
        {
            Calculator calc = new Calculator();

            var result = calc.Calculate(data);

            AnalysisModule.MessageBus.Instance.PublishResult(result);
        }
    }
}
