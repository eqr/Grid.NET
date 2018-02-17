using System;
using System.Linq.Expressions;

namespace Grid.Cache.Indexing
{
    public class Indexer<T, P>
    {
        private Expression<Func<T, P>> indexExpression;
        private Func<T, P> indexFunction;

        public Indexer(){}
        
        public Indexer(Expression<Func<T, P>> indexExpression)
        {
            this.indexExpression = indexExpression;
            this.indexFunction = indexExpression.Compile();
        }

        public P Index(T element)
        {
            return this.indexFunction(element);
        }

        public Expression<Func<T, P>> GetAccessorExpression()
        {
            return this.indexExpression;
        }

        public Func<T, P> GetAccessorFunction()
        {
            return this.indexFunction;
        }
    }
}