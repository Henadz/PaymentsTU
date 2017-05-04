using FrameworkExtend;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PaymentsTU.Database
{
	internal sealed class WhereTranslator : ExpressionVisitor
	{
		StringBuilder sb;



		internal WhereTranslator()
		{

		}



		internal string Translate(Expression expression)
		{

			this.sb = new StringBuilder("WHERE ");

			this.Visit(expression);

			return this.sb.ToString();

		}



		private static Expression StripQuotes(Expression e)
		{
			while (e.NodeType == ExpressionType.Quote)
			{
				e = ((UnaryExpression)e).Operand;
			}

			return e;
		}



		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			//TODO: support 
			//if (/*m.Method.DeclaringType == typeof(Queryable) &&*/ m.Method.Name == "Where") {

			//	sb.Append("SELECT * FROM(");

			//	this.Visit(m.Arguments[0]);

			//	sb.Append(") AS T WHERE ");

			//	LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

			//	this.Visit(lambda.Body);

			//	return m;

			//}
			if (m.Method == typeof(string).GetMethod("Trim", new Type[0] ))
			{
				sb.Append(" TRIM(");
				this.Visit(m.Object);
				sb.Append(")");
				return m;
			}

			throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));

		}



		protected override Expression VisitUnary(UnaryExpression u)
		{
			switch (u.NodeType)
			{
				case ExpressionType.Not:
					sb.Append(" NOT ");
					this.Visit(u.Operand);
					break;

				default:
					throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
			}

			return u;
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			sb.Append("(");
			this.Visit(b.Left);

			switch (b.NodeType)
			{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					sb.Append(" AND ");
					break;

				case ExpressionType.Or:
					sb.Append(" OR ");
					break;

				case ExpressionType.Equal:
					sb.Append(IsNullValue(b.Right) ? " IS " : " = ");
					break;

				case ExpressionType.NotEqual:
					sb.Append(IsNullValue(b.Right) ? " IS NOT " : " <> ");
					break;

				case ExpressionType.LessThan:
					sb.Append(" < ");
					break;

				case ExpressionType.LessThanOrEqual:
					sb.Append(" <= ");
					break;

				case ExpressionType.GreaterThan:
					sb.Append(" > ");
					break;

				case ExpressionType.GreaterThanOrEqual:
					sb.Append(" >= ");
					break;

				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
			}

			this.Visit(b.Right);
			sb.Append(")");

			return b;
		}

		private static bool IsNullValue(Expression expression)
		{
			var c = expression as ConstantExpression;
			return c != null && c.Value == null;
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			AppendValue(c.Value);
			return c;
		}



		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Expression != null)
			{
				if (m.Expression.NodeType == ExpressionType.Parameter)
				{
					var meta = (DBMetadata)m.Member.GetCustomAttributes(typeof(DBMetadata), false).FirstOrDefault();
					if (meta != null)
					{
						if (!string.IsNullOrEmpty(meta.TableName?.Trim()))
							sb.Append($"{meta.TableName}.");
						if (!string.IsNullOrEmpty(meta.ColumnName?.Trim()))
							sb.Append(meta.ColumnName);
						else
							sb.Append(m.Member.Name);
					}
					else
						sb.Append(m.Member.Name);
					return m;
				}

				if (m.Expression.NodeType == ExpressionType.MemberAccess)
				{
					if (m.Member is FieldInfo || m.Member is PropertyInfo)
					{
						AppendValue(GetValue(m));
						return m;
					}
				}

				if (m.Expression.NodeType == ExpressionType.Constant)
				{
					AppendValue(GetValue(m));
					return m;
				}
			}

			throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));

		}

		private static object GetValue(Expression member)
		{
			// source: http://stackoverflow.com/a/2616980/291955
			var objectMember = Expression.Convert(member, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember);
			var getter = getterLambda.Compile();
			return getter();
		}

		private void AppendValue(object value)
		{
			if (value == null)
			{
				sb.Append("NULL");
			}
			else
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Boolean:
						sb.Append(((bool)value) ? 1 : 0);
						break;
					case TypeCode.String:
						sb.Append("'");
						sb.Append(value);
						sb.Append("'");
						break;
					case TypeCode.Object:
						throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
					default:
						sb.Append(value);
						break;
				}
			}
		}

	}
}
