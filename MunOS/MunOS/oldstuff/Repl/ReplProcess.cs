//using System;
//using System.Collections.Generic;

//namespace MunOS.Repl
//{
//	public class ReplProcess : ROProcess
//	{
//		IReplEngine engine;

//		public ReplProcess(IReplEngine engine)
//		{
//			this.engine = engine;
//			engine.SetReplProcess(this);
//		}

//		enum EvaluationType
//		{
//			FILENAME,
//			SOURCE
//		}

//		struct Evaluation
//		{
//			public string filename;
//			public string source;
//			public EvaluationType evaluationType;
//		}

//		Queue<Evaluation> evaluationQueue = new Queue<Evaluation>();
//		Queue<string> inputQueue = new Queue<string>();

//		public IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
//		{
//			return engine.GetCompletions(source,cursorPos,out replaceStart,out replaceEnd);
//		}

//		public IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
//		{
//			return engine.GetDisplayableCompletions(source, cursorPos, out replaceStart, out replaceEnd);
//		}

//		public void ResetProcess()
//		{
//			inputQueue.Clear();
//			evaluationQueue.Clear();
//			engine.Reset();
//		}

//		public void PrintReturnValue(string returnValueString)
//		{
//			OutputBuffer.Print("\nr> " + returnValueString);
//		}

//		public void PrintOutput(string outputString)
//		{
//			OutputBuffer.Print("\no> " + outputString);
//		}

//		public void PrintError(string errorString)
//		{
//			OutputBuffer.Print("\ne> " + errorString);
//		}

//		public void PrintInput(string inputString)
//		{
//			OutputBuffer.Print("\ni> " + inputString);
//		}

//		public void PrintSource(string source)
//		{
//			OutputBuffer.Print("\ns> " + source);
//		}

//		public void PrintFilename(string filename)
//		{
//			OutputBuffer.Print("\nf> " + filename);
//		}

//		public ReplHistory InputHistory = new ReplHistory();
//		public void ReplInput(string str,bool withHistory=false)
//		{
//			if (withHistory)
//			{
//				InputHistory.Add(str);
//			}

//			inputQueue.Enqueue(str);
//		}

//		public ReplHistory SourceHistory = new ReplHistory();
//		public void EvaluateSource(string source,bool withHistory=false)
//		{
//			if (withHistory)
//			{
//				SourceHistory.Add(source);
//			}

//			evaluationQueue.Enqueue(new Evaluation { source = source, evaluationType=EvaluationType.SOURCE });
//		}

//		public void EvaluateFile(string filename)
//		{
//			evaluationQueue.Enqueue(new Evaluation { filename = filename,evaluationType=EvaluationType.FILENAME });
//		}

//		protected override bool ProtectedFixedUpdate(float timeAllotted)
//		{
//			if (engine.IsIdle())
//			{
//				if (evaluationQueue.Count != 0)
//				{
//					var evaluation = evaluationQueue.Dequeue();

//					var source = "";
//					if (evaluation.evaluationType == EvaluationType.FILENAME)
//					{
//						PrintFilename(evaluation.filename);
//						try
//						{
//							source = IOUtility.ReadScript(evaluation.filename);
//						}
//						catch (Exception ex)
//						{
//							PrintError(ex.Message+". while loading file " + evaluation.filename);
//							return false;
//						}
//					}
//					else
//					{
//						PrintSource(evaluation.source);
//						source = evaluation.source;
//					}

//					engine.EvaluateSource(source);
//				}
//			}
//			else if(engine.IsWaitingForInput() && inputQueue.Count>0)
//			{
//				engine.ReceiveInput(inputQueue.Dequeue());
//			}

//			engine.FixedUpdate(timeAllotted);

//			return false;
//		}

//		/// <summary>
//		/// Interrupt the current evaluations. And clear incoming input.
//		/// </summary>
//		public void Interrupt()
//		{
//			engine.Interrupt();
//			inputQueue.Clear();
//			evaluationQueue.Clear();
//		}

//		public override void ReceiveInput(string str)
//		{
//			inputQueue.Enqueue(str);
//		}

//		public override bool IsWaitingForInput()
//		{
//			return inputQueue.Count == 0 && engine.IsWaitingForInput();
//		}

//		public bool IsIdle()
//		{
//			return engine.IsIdle() && evaluationQueue.Count == 0;
//		}
//	}
//}
