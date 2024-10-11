using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFAnimations
{
    /// <summary>
    /// Animation manager
    /// </summary>
    [ProvideProperty("Decoration", typeof(System.Windows.Forms.Control))]
    public class Animator : Component, IExtenderProvider
    {
        IContainer components = null;
        protected List<QueueItem> queue = new List<QueueItem>();
        private Thread thread;
        System.Windows.Forms.Timer timer;


    }
}
