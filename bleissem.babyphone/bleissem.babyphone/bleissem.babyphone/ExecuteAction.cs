using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class ExecuteAction : IExecuteAction
    {

        public ExecuteAction(Action action)
        {
            m_Action = action;
        }

        ~ExecuteAction()
        {
            this.Dispose(false);
        }


        private Action m_Action;

        private void Dispose(bool disposing)
        {
            if (null != m_Action)
            {
                m_Action = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Execute()
        {
            m_Action();
        }
    }


    public class ExecuteGenericAction<T>: IExecuteGenericAction<T>
    {

        public ExecuteGenericAction(Action<T> action)
        {
            m_Action = action;
        }

        ~ExecuteGenericAction()
        {
            this.Dispose(false);
        }


        private Action<T> m_Action;

        private void Dispose(bool disposing)
        {
            if(null != m_Action)
            {
                m_Action = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private T m_Value;

        public void SetValue(T value)
        {
            m_Value = value;            
        }

        public void Execute()
        {
            m_Action(m_Value);
        }
    }
    
}
