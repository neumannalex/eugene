using Eugene.Core.Helper;
using Eugene.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core
{
    public class BlockerResolver
    {
        private TestcaseBlockerDataset _initialDataset;
        public TestcaseBlockerDataset InitialDataset
        {
            get
            {
                return _initialDataset;
            }
        }

        private List<ExtendedBlocker> _extendedBlockers = new List<ExtendedBlocker>();

        public BlockerResolver(TestcaseBlockerDataset initialDataset)
        {
            _initialDataset = initialDataset;

            _extendedBlockers = InitialDataset.Blockers.Select(x => new ExtendedBlocker
            {
                Id = x.Id,
                Name = x.Name,
                Cost = x.Cost,
                Testcases = InitialDataset.Testcases.Where(t => t.BlockerIds.Contains(x.Id)).ToList()
            }).ToList();
        }

        public BlockerResolverResult Resolve(List<Blocker> blockers)
        {
            var blockerIdsToResolve = blockers.Select(x => x.Id);

            var unresolvedBlockers = _extendedBlockers.Where(x => !blockerIdsToResolve.Contains(x.Id)).Select(x => new Blocker
            {
                Id = x.Id,
                Name = x.Name,
                Cost = x.Cost
            });

            var resolvedTestcases = InitialDataset.Testcases.Select(x => new Testcase
            {
                Id = x.Id,
                Name = x.Name,
                TestType = x.TestType,
                ApplicationModule = x.ApplicationModule,
                BlockerIds = x.BlockerIds.Except(blockerIdsToResolve).ToList()
            });

            var localDataset = new TestcaseBlockerDataset
            {
                Blockers = unresolvedBlockers.ToList(),
                Testcases = resolvedTestcases.ToList()
            };

            var result = new BlockerResolverResult(_initialDataset, localDataset);

            return result;
        }
    }

    public class ExtendedBlocker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }

        public ICollection<Testcase> Testcases { get; set; } = new TestcaseCollection();
    }

    public class TestcaseCollection : ICollection<Testcase>
    {
        private Dictionary<string, Testcase> _innerCollection;

        public TestcaseCollection()
        {
            _innerCollection = new Dictionary<string, Testcase>();
        }

        public int Count
        {
            get
            {
                return _innerCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public Testcase this[string id]
        {
            get { return _innerCollection[id]; }
            set { _innerCollection[id] = value; }
        }

        public void Add(Testcase item)
        {
            if(!Contains(item))
            {
                _innerCollection.Add(item.Id, item);
            }
        }

        public void Clear()
        {
            _innerCollection.Clear();
        }

        public bool Contains(Testcase item)
        {
            return _innerCollection.ContainsKey(item.Id);
        }

        public void CopyTo(Testcase[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();

            if(Count > array.Length - arrayIndex + 1)
                throw new ArgumentOutOfRangeException();

            for(int i = 0; i < _innerCollection.Count; i++)
            {
                array[i + arrayIndex] = _innerCollection.ElementAt(i).Value;
            }
        }

        public IEnumerator<Testcase> GetEnumerator()
        {
            return new TestcaseEnumerator(this);
        }

        public bool Remove(Testcase item)
        {
            if (Contains(item))
            {
                return _innerCollection.Remove(item.Id);
            }
            else
            {
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TestcaseEnumerator(this);
        }
    }

    public class TestcaseEnumerator : IEnumerator<Testcase>
    {
        private TestcaseCollection _collection;
        private int _curIndex;
        private Testcase _currentTestcase;

        public TestcaseEnumerator(TestcaseCollection collection)
        {
            _collection = collection;
            _curIndex = -1;
            _currentTestcase = default(Testcase);
        }

        public Testcase Current
        {
            get { return _currentTestcase; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if(++_curIndex >= _collection.Count)
            {
                return false;
            }
            else
            {
                _currentTestcase = _collection.ElementAt(_curIndex);
            }
            return true;
        }

        public void Reset()
        {
            _curIndex = -1;
        }
    }
}
