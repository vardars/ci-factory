using System;

namespace ObjectWizard
{
	public class DecoratedType : TypeDecoratorable
	{
		private readonly Type type;
		private DecoratedType decorator;

		public DecoratedType(Type type)
		{
			this.type = type;
		}

		public TypeDecoratorable Decorate(Type type)
		{
			decorator = new DecoratedType(type);
			return decorator;
		}

		public Type Type
		{
			get { return type; }
		}

		public DecoratedType Decorator
		{
			get { return decorator; }
		}
	}
}
