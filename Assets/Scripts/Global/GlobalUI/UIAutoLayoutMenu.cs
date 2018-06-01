using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Global.GlobalUI
{
    public class UIAutoLayoutMenu : UIMenu
    {
        public virtual float DoLayout()
        {
            UnLockLayout();
            return -1;
        }
        public virtual void LockLayout()
        {

        }
        public virtual void UnLockLayout()
        {

        }
    }
}
