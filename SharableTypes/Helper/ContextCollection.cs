using System.Collections.Generic;
using SharableTypes.Model;

namespace SharableTypes.Helper
{
    public class ContextCollection
    {
        private int _index = -1000000;
        private IDictionary<int, QuestionContextM> _contextList;

        public int AddToCollection(QuestionContextM context)
        {
            var tagId = -1;
            if (_contextList == null)
            {
                _contextList = new Dictionary<int, QuestionContextM>();
            }

            if (context != null)
            {
                tagId = _index++;
                _contextList.Add(tagId, context);
            }

            return tagId;
        }

        public QuestionContextM GetFromCollection(int tagId)
        {
            return _contextList != null && tagId != 0 ? _contextList[tagId] : null;
        }
    }
}
