using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZENC.SAMPLE.OFFICE.OUTLOOK
{
    public delegate void OneParameterEventDelegate(object o);

    /// <summary>
    /// 여러개의 parameter 값을 가지는 이벤트 핸들러입니다.
    /// </summary>
    /// <param name="o">반환값</param>
    public delegate void MultiParameterEventDelegate(object[] o);

    public delegate void NoneParameterEventDelegate();
    public delegate bool NoneParameterReturnBoolDelegate();
    public delegate string[] BoolParameterReturnStringArrayDelegate(bool o);
    public delegate void BoolParameterDelegate(bool o);
    public delegate void ExceptionParameterHandler(Exception ex);
    //public delegate void IEzTaskParameterHandler(IEzTask task);
    public delegate void StringParameterHandler(string str);
    public delegate void ObjectParameterHandler(object obj);
    public delegate object GetReturnObjectHandler();
    public delegate bool GetReturnBoolHandler();
    public delegate bool GetReturnBoolObjectParam(object param);
    public delegate bool GetReturnBoolLongParam(long param);
    public delegate byte[] GetReturnByteByteParam(byte[] param);
    public delegate byte[] GetReturnByteLongParam(long size);
    public delegate void GetByteParamHandler(byte[] param);
    public class EzRunAsyncAndCompleteSyncTaskBase
    {
        public virtual void RunHandler() { }
        public virtual void CompleteHandler() { }
        public virtual void ErrorHandler(Exception ex) { }
    }

    public class EzRunAsyncAndCompleteSyncTask : EzRunAsyncAndCompleteSyncTaskBase
    {
        NoneParameterEventDelegate asyncHandler;
        NoneParameterEventDelegate completeHandler;
        ExceptionParameterHandler errorHandler;
        /// <param name="o">반환값</param>
      

        public EzRunAsyncAndCompleteSyncTask(NoneParameterEventDelegate asyncHandler,
            NoneParameterEventDelegate completeHandler,
            ExceptionParameterHandler errorHandler)
        {
            this.asyncHandler = asyncHandler;
            this.completeHandler = completeHandler;
            this.errorHandler = errorHandler;
        }
        public EzRunAsyncAndCompleteSyncTask(NoneParameterEventDelegate asyncHandler)
            : this(asyncHandler, null, null)
        {

        }

        public EzRunAsyncAndCompleteSyncTask(NoneParameterEventDelegate asyncHandler,
            ExceptionParameterHandler errorHandler)
            : this(asyncHandler, null, errorHandler)
        {

        }
        public EzRunAsyncAndCompleteSyncTask(NoneParameterEventDelegate asyncHandler,
           NoneParameterEventDelegate completeHandler)
            : this(asyncHandler, completeHandler, null)
        {

        }
        public override void RunHandler()
        {
            if (asyncHandler != null)
                asyncHandler();
        }

        public override void ErrorHandler(Exception ex)
        {
            if (errorHandler!=null)
                errorHandler(ex);
            else if (completeHandler != null)
                completeHandler();
        }
        public override void CompleteHandler()
        {
            if (completeHandler != null)
                completeHandler();
        }
    }

    public class EzRunAsyncAndCompleteSyncTask2 : EzRunAsyncAndCompleteSyncTaskBase
    {
        ObjectParameterHandler asyncHandler;
        NoneParameterEventDelegate completeHandler;
        ExceptionParameterHandler errorHandler;
        object obj;

        public EzRunAsyncAndCompleteSyncTask2(ObjectParameterHandler asyncHandler, object obj)
            : this(asyncHandler, null, null, obj)
        {
        }

        public EzRunAsyncAndCompleteSyncTask2(ObjectParameterHandler asyncHandler,
            ExceptionParameterHandler errorHandler, object obj)
            : this(asyncHandler, null, errorHandler, obj)
        {
        }

        public EzRunAsyncAndCompleteSyncTask2(ObjectParameterHandler asyncHandler,
            NoneParameterEventDelegate completeHandler, ExceptionParameterHandler errorHandler, object obj)
        {
            this.asyncHandler = asyncHandler;
            this.completeHandler = completeHandler;
            this.errorHandler = errorHandler;
            this.obj = obj;
        }

        public override void RunHandler()
        {
            if (asyncHandler != null)
                asyncHandler(obj);
        }

        public override void ErrorHandler(Exception ex)
        {
            if (errorHandler != null)
                errorHandler(ex);
        }
        public override void CompleteHandler()
        {
            if (completeHandler != null)
                completeHandler();
        }
    }

    public class EzRunAsyncAndCompleteSyncTasks : IDisposable
    {
        
        bool suspend = false;
        BackgroundWorker worker = new BackgroundWorker();
        EzRunAsyncAndCompleteSyncTaskBase currentTask;
        Queue<EzRunAsyncAndCompleteSyncTaskBase> tasks = new Queue<EzRunAsyncAndCompleteSyncTaskBase>();
        private static EzRunAsyncAndCompleteSyncTasks instance = null;
        private static object instanceLock = "lock";
        public static EzRunAsyncAndCompleteSyncTasks Instance
        {
            get
            {
                //여러개의 쓰레드에서 접근 할 때 생기는 문제로 인하여 이 부분을 락 걸었습니다. //유원상 //2016-05-09
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new EzRunAsyncAndCompleteSyncTasks();
                        instance.InstanceID = Guid.NewGuid();
                        instanceList.Add(instance.InstanceID, instance);
                    }
                }

                return instance;
            }
        }
        private static Dictionary<Guid, EzRunAsyncAndCompleteSyncTasks> instanceList = new Dictionary<Guid, EzRunAsyncAndCompleteSyncTasks>();
        public Guid InstanceID { get; set; }
        /// <summary>
        /// 해당 Instance의 Task가 모두 끝났는지에 대한 여부를 알 수 있습니다. //유원상 //2015-02-25
        /// </summary>
        public bool IsTaskFinished { get; set; }
        public static EzRunAsyncAndCompleteSyncTasks GetInstance(Guid instanceID)
        {
            if (instanceList == null)
            {
                instanceList = new Dictionary<Guid, EzRunAsyncAndCompleteSyncTasks>();
            }

            if (!instanceList.ContainsKey(instanceID))
            {
                EzRunAsyncAndCompleteSyncTasks nI = new EzRunAsyncAndCompleteSyncTasks();
                nI.InstanceID = instanceID;
                instanceList.Add(nI.InstanceID, nI);
            }
            return instanceList[instanceID];

        }

        public int Count()
        {
            return tasks.Count;
        }
        EzRunAsyncAndCompleteSyncTaskBase GetTask()
        {
            if (tasks.Count > 0)
            {
                return tasks.Dequeue();
            }

            return null;
        }

        private EzRunAsyncAndCompleteSyncTasks()
        {
            IsTaskFinished = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (sender, arg) =>
            {
                IsTaskFinished = false;
                if (currentTask != null)
                {
                    currentTask.RunHandler();
                }
            };

            worker.RunWorkerCompleted += (sender, arg) =>
            {
                if (currentTask != null)
                {
                    if (tasks.Count == 0)
                        IsTaskFinished = true;

                    if (arg.Error !=null)
                    {
                        currentTask.ErrorHandler(arg.Error);
                    }
                    else
                    {
                        currentTask.CompleteHandler();
                    }
                    if (suspend)
                        return;

                    currentTask = GetTask();

                    if (currentTask != null)
                        worker.RunWorkerAsync();
                }
            };



        }

        public void AddTask(NoneParameterEventDelegate asyncHandler,
        NoneParameterEventDelegate completeHandler,
        ExceptionParameterHandler errorHandler)
        {
            AddTask(new EzRunAsyncAndCompleteSyncTask(asyncHandler, completeHandler, errorHandler));
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler)
        {
            AddTask(new EzRunAsyncAndCompleteSyncTask(asyncHandler));
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler, NoneParameterEventDelegate completeHandler)
        {

            AddTask(new EzRunAsyncAndCompleteSyncTask(asyncHandler, completeHandler));
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler, ExceptionParameterHandler errorHandler)
        {
            AddTask(new EzRunAsyncAndCompleteSyncTask(asyncHandler, errorHandler));
        }

        private object isLock = "lock";
        public void AddTask(EzRunAsyncAndCompleteSyncTaskBase task)
        {
            //여러개의 쓰레드에서 add task를 할 때 생기는 문제로 인하여 이 부분을 락 걸었습니다. //유원상 //2016-05-09
            lock (isLock)
            {
                tasks.Enqueue(task);

                if (currentTask == null && !suspend && !worker.IsBusy)
                {
                    currentTask = GetTask();
                    worker.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// 진행중이던 작업까지만 완료하고 큐에 대기중인 작업을 일시중지합니다.
        /// </summary>
        public void SuspendTask()
        {
            SuspendTask(false);
        }

        /// <summary>
        /// 큐에 대기중인 작업을 일시중지합니다. isForce가 true일 경우 진행중이던 worker에 CancelAsync를 날립니다. false일 경우 진행하던 작업까지는 진행을 완료합니다.
        /// </summary>
        /// <param name="isForce"></param>
        public void SuspendTask(bool isForce)
        {
            if (isForce)
                worker.CancelAsync();

            suspend = true;
        }

        public void ReStartTask()
        {
            suspend = false;
            if (currentTask == null)
            {
                currentTask = GetTask();
                worker.RunWorkerAsync();
            }
        }

        public void CancelTasks()
        {
            tasks.Clear();
        }


        public void Dispose()
        {
            if (worker != null) worker.Dispose();
        }
    }


    /// <summary>
    /// 하나의 인스턴스(하나의 대기 큐)를 공유하는 여러개의 타스크가 필요할 때 사용합니다. //유원상 //2016-03-08
    /// </summary>
    public class EzRunAsyncAndCompleteSyncMultiTasks : IDisposable
    {
        private bool useCpuWatch = false;
        private bool keepGoing = true;
        private PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        private double CurrentCpuPercent;

        public int CurrentMaxCount = 4;

        private bool isFlag = true;
        /// <summary>
        /// 2초에 한번씩 cpu 사용량을 가져옵니다. // 한태광 / 2016.3.9
        /// </summary>
        private void CpuCheck()
        {
            do
            {
                CurrentCpuPercent = Math.Round(cpu.NextValue(), 2);


                if (useCpuWatch && isFlag)
                {
                    isFlag = false;
                    // cpu 감시 기능이 동작중인 상태에서 현재치를 체크 후 업무의 강도를 조정합니다. // 한태광 / 2016.3.9
                    if (CurrentCpuPercent > 90 && CurrentMaxCount > 1) // 1 밑으로는 떨어지지 않게합니다.
                    {
                        SetMaxCount(CurrentMaxCount - 1);
                    }
                    else if (CurrentCpuPercent <= 60 && CurrentMaxCount < 10) // 최대치 10으로 고정..
                    {
                        SetMaxCount(CurrentMaxCount + 1);
                    }
                    isFlag = true;
                }

                Thread.Sleep(5000);
            }
            while (useCpuWatch && keepGoing);
        }

        bool suspend = false;
        List<Guid> threadIDList = new List<Guid>();
        Dictionary<Guid, BackgroundWorker> workerList = new Dictionary<Guid, BackgroundWorker>();
        Dictionary<Guid, EzRunAsyncAndCompleteSyncTaskBase> taskList = new Dictionary<Guid, EzRunAsyncAndCompleteSyncTaskBase>();
        Queue<EzRunAsyncAndCompleteSyncTaskBase> tasks = new Queue<EzRunAsyncAndCompleteSyncTaskBase>();

        private static Dictionary<Guid, EzRunAsyncAndCompleteSyncMultiTasks> instanceList;

        public Guid InstanceID { get; set; }
        /// <summary>
        /// 해당 Instance의 Task가 모두 끝났는지에 대한 여부를 알 수 있습니다. //유원상 //2015-02-25
        /// </summary>
        public bool IsTaskFinished { get; set; }


        /// <summary>
        /// 동시에 도는 스레드 카운트를 설정합니다. 디폴트로 4개로 지정되어있습니다.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public static void SetMaxCount(Guid instanceID, int maxCount)
        {
            GetInstance(instanceID).SetMaxCount(maxCount);
        }

        void SetMaxCount(int maxCount)
        {
            //최초에는 전부 ADD
            if (threadIDList.Count == 0)
            {
                for (int i = 0; i < maxCount; i++)
                    threadIDList.Add(Guid.NewGuid());
            }
            else
            {
                if (CurrentMaxCount > maxCount) //혹시 줄어들어야 할 경우는 삭제
                {
                    for (int i = maxCount; i < CurrentMaxCount; i++)
                    {
                        threadIDList.RemoveAt(0);
                    }
                }
                else if (CurrentMaxCount < maxCount) //혹시 늘려야 할 경우는 추가
                {
                    for (int i = CurrentMaxCount; i < maxCount; i++)
                    {
                        Guid guid = Guid.NewGuid();
                        threadIDList.Add(guid);

                        //if (!suspend)
                        //{
                        //    taskList.Add(guid, GetTask());
                        //    workerList.Add(guid, GetBackgroundWorker(guid));
                        //    workerList[guid].RunWorkerAsync();
                        //}
                    }
                }
            }
            CurrentMaxCount = maxCount;
        }

        /// <summary>
        /// 멀티 타스크입니다. 동시에(병렬로) 도는 스레드 카운트를 지정하려면 SetMaxCount메서드를 호출하십시오. 기본은 4개로 지정되어있습니다. 너무 많이하면 오히려 느릴 수 있습니다.
        /// </summary>
        public static EzRunAsyncAndCompleteSyncMultiTasks Instance
        {
            get
            {

                if (instanceList == null)
                {
                    instanceList = new Dictionary<Guid, EzRunAsyncAndCompleteSyncMultiTasks>();
                }

                if (instance == null)
                {
                    instance = new EzRunAsyncAndCompleteSyncMultiTasks();
                    instance.InstanceID = Guid.NewGuid();
                    instanceList.Add(instance.InstanceID, instance);
                }

                return instance;
            }
        }
        private static EzRunAsyncAndCompleteSyncMultiTasks instance;


        /// <summary>
        /// 멀티 타스크입니다. 동시에(병렬로) 도는 스레드 카운트를 지정하려면 SetMaxCount메서드를 호출하십시오. 기본은 4개로 지정되어있습니다. 너무 많이하면 오히려 느릴 수 있습니다.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static EzRunAsyncAndCompleteSyncMultiTasks GetInstance(Guid instanceID)
        {
            if (instanceList == null)
            {
                instanceList = new Dictionary<Guid, EzRunAsyncAndCompleteSyncMultiTasks>();
            }

            if (!instanceList.ContainsKey(instanceID))
            {
                EzRunAsyncAndCompleteSyncMultiTasks nI = new EzRunAsyncAndCompleteSyncMultiTasks();
                nI.InstanceID = instanceID;
                instanceList.Add(instanceID, nI);
            }
            return instanceList[instanceID];
        }

        public int Count()
        {
            return tasks.Count;
        }

        private EzRunAsyncAndCompleteSyncTaskBase GetTask()
        {
            lock (this)
            {
                try
                {
                    if (tasks.Count > 0)
                    {
                        return tasks.Dequeue();
                    }

                    return null;
                }
                catch //(Exception ex)
                {
                    //ex.EzLog();
                    return null;
                }
            }
        }

        private EzRunAsyncAndCompleteSyncMultiTasks()
        {
            IsTaskFinished = true;
            SetMaxCount(CurrentMaxCount);
        }

        public void AddTask(NoneParameterEventDelegate asyncHandler,
        NoneParameterEventDelegate completeHandler,
        ExceptionParameterHandler errorHandler)
        {
            AddTask(asyncHandler, completeHandler, errorHandler, false);
        }

        public void AddTask(NoneParameterEventDelegate asyncHandler)
        {
            AddTask(asyncHandler, null, null, false);
        }

        public void AddTask(NoneParameterEventDelegate asyncHandler, NoneParameterEventDelegate completeHandler)
        {
            AddTask(asyncHandler, completeHandler, null, false);
        }

        public void AddTask(NoneParameterEventDelegate asyncHandler, ExceptionParameterHandler errorHandler)
        {
            AddTask(asyncHandler, null, errorHandler, false);
        }
        Thread cpuUsageMonitorThread;
        public void AddTask(NoneParameterEventDelegate asyncHandler,
        NoneParameterEventDelegate completeHandler,
        ExceptionParameterHandler errorHandler, bool useCpuWatch)
        {
            AddTask(new EzRunAsyncAndCompleteSyncTask(asyncHandler, completeHandler, errorHandler));
            // cpu 감시 기능을 동작합니다.
            this.useCpuWatch = useCpuWatch;
            if (this.useCpuWatch && !(cpuUsageMonitorThread!=null&& cpuUsageMonitorThread.IsAlive))
            {
                cpuUsageMonitorThread = new Thread(CpuCheck);
                cpuUsageMonitorThread.Start();
            }
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler, bool useCpuWatch)
        {
            AddTask(asyncHandler, null, null, useCpuWatch);
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler, NoneParameterEventDelegate completeHandler, bool useCpuWatch)
        {
            AddTask(asyncHandler, completeHandler, null, useCpuWatch);
        }
        public void AddTask(NoneParameterEventDelegate asyncHandler, ExceptionParameterHandler errorHandler, bool useCpuWatch)
        {
            AddTask(asyncHandler, null, errorHandler, useCpuWatch);
        }

        public void AddTask(EzRunAsyncAndCompleteSyncTaskBase task)
        {
            tasks.Enqueue(task);

            if (!suspend)
            {
                foreach (Guid guid in threadIDList)
                {
                    if (!taskList.ContainsKey(guid))
                    {
                        taskList.Add(guid, GetTask());
                        workerList.Add(guid, GetBackgroundWorker(guid));
                        workerList[guid].RunWorkerAsync();
                        break;
                    }
                    else if (taskList[guid] == null)
                    {
                        taskList[guid] = GetTask();
                        workerList[guid].RunWorkerAsync();
                        break;
                    }
                }
            }
        }

        private BackgroundWorker GetBackgroundWorker(Guid guid)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (sender, arg) =>
            {
                IsTaskFinished = false;
                if (taskList.ContainsKey(guid) && taskList[guid] == null)
                {
                    taskList[guid].RunHandler();
                }
            };

            worker.RunWorkerCompleted += (sender, arg) =>
            {
                if (taskList.ContainsKey(guid) && taskList[guid] == null)
                {
                    int notnullCnt = taskList.Values.ToList().Count(c => c != null);
                    //마지막으로 돌고있는 타스크가 한개일 경우 마지막 타스크의 CompleteHandler이벤트가 탈 때 true가 될 수 있도록 합니다. //유원상 //2016.03.29
                    if (notnullCnt == 1 && tasks.Count == 0)
                        IsTaskFinished = true;

                    if (arg.Error == null)
                    {
                        taskList[guid].ErrorHandler(arg.Error);
                    }
                    else
                    {
                        taskList[guid].CompleteHandler();
                    }

                    if (suspend)
                        return;

                    if (threadIDList.Contains(guid))
                    {
                        taskList[guid] = GetTask();

                        if (taskList[guid] != null)
                        {
                            worker.RunWorkerAsync();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        taskList.Remove(guid);
                        workerList[guid].Dispose();
                        workerList.Remove(guid);
                    }
                }
            };
            return worker;
        }

        /// <summary>
        /// 진행중이던 작업까지만 완료하고 큐에 대기중인 작업을 일시중지합니다.
        /// </summary>
        public void SuspendTask()
        {
            SuspendTask(false);
        }

        /// <summary>
        /// 큐에 대기중인 작업을 일시중지합니다. isForce가 true일 경우 진행중이던 worker에 CancelAsync를 날립니다. false일 경우 진행하던 작업까지는 진행을 완료합니다.
        /// </summary>
        /// <param name="isForce"></param>
        public void SuspendTask(bool isForce)
        {
            if (isForce)
            {
                foreach (BackgroundWorker worker in workerList.Values)
                    worker.CancelAsync();
            }

            suspend = true;
        }

        public void ReStartTask()
        {
            suspend = false;
            foreach (Guid guid in threadIDList)
            {
                if (!taskList.ContainsKey(guid))
                {
                    taskList.Add(guid, GetTask());
                    workerList.Add(guid, GetBackgroundWorker(guid));
                    workerList[guid].RunWorkerAsync();
                }
                else if (taskList[guid]==null)
                {
                    taskList[guid] = GetTask();
                    workerList[guid].RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// 진행되던것 까지 진행하고 남은 작업을 모두 클리어 시킵니다.
        /// </summary>
        public void CancelTasks()
        {
            CancelTasks(false);
        }

        /// <summary>
        /// 진행되던것을 캔슬시키고 남은 작업을 모두 클리어 시킵니다.
        /// </summary>
        /// <param name="isForce"></param>
        public void CancelTasks(bool isForce)
        {
            if (isForce)
            {
                foreach (BackgroundWorker worker in workerList.Values)
                    worker.CancelAsync();
            }
            tasks.Clear();
        }

        public void Dispose()
        {
            if (useCpuWatch && keepGoing)
            {
                useCpuWatch = keepGoing = false; // cpu 감시를 중지합니다. // 한태광 / 2016.3.9
            }
            if (workerList.Count > 0)
            {
                foreach (BackgroundWorker worker in workerList.Values)
                    worker.Dispose();
            }
        }
    }

    /// <summary>
    /// Run, Stop기능이 있는 Thread입니다. / 김태혁 / 2016.3.18
    /// </summary>
    public class EzThreadAsyncTask
    {
        EzRunAsyncAndCompleteSyncTaskBase currentTask;
        public EzThreadAsyncTask(EzRunAsyncAndCompleteSyncTaskBase currentTask)
        {
            this.currentTask = currentTask;
        }
        System.Threading.Thread thread;

        public void Run()
        {

            thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {

                bool isError = false;

                try
                {

                    if (currentTask != null)
                    {
                        currentTask.RunHandler();
                    }


                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    isError = true;
                    if (currentTask != null)
                        currentTask.ErrorHandler(ex);
                }
                catch (InvalidOperationException ex)
                {
                    isError = true;
                    if (currentTask != null)
                        currentTask.ErrorHandler(ex);
                }
                catch (Exception ex)
                {
                    isError = true;
                    if (currentTask != null)
                        currentTask.ErrorHandler(ex);
                }
                finally
                {
                    if (!isError && currentTask != null)
                        currentTask.CompleteHandler();

                    thread = null;

                }
            }));
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.IsBackground = true;

            thread.Start();
        }

        public void Stop()
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }

    }
}
