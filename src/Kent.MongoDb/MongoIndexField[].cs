namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     Represents a type for Mongo index field with a generic type.
    /// </summary>
    /// <typeparam name="TDocument">The document type.</typeparam>
    public class MongoIndexField<TDocument> : MongoIndexField
    {
        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="expression">The expression for the field.</param>
        /// <param name="indexType">The index type.</param>
        public MongoIndexField(Expression<Func<TDocument, object>> expression, MongoIndexType indexType = MongoIndexType.Ascending) : base(null, indexType)
        {
            var expressionBody = expression.Body;
            var member = expressionBody as MemberExpression ?? ((UnaryExpression)expressionBody).Operand as MemberExpression;
            this.Name = GetExpressionName(member);
        }

        private static string GetExpressionName(MemberExpression member)
        {
            string name = string.Empty;
            if (member.Expression is MemberExpression memberExpression)
            {
                name = $"{GetExpressionName(memberExpression)}.";
            }
            return $"{name}{member.Member.Name}";
        }
    }
}