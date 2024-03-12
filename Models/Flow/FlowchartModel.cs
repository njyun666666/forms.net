using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class FlowchartModel
    {
        public string formClass { get; set; }
        public string flowID { get; set; }
        public int? stepID { get; set; }
    }
    public class FlowchartViewModel
    {
        public List<FlowchartStepModel> Step { get; set; }
        public List<FlowchartLineModel> Line { get; set; }
    }
    public class FlowchartStepModel
    {
        public int StepID { get; set; }
        public int StepType { get; set; }
        public string StepName { get; set; }
    }
    public class FlowchartLineModel
    {
        public int StartStepID { get; set; }
        public int EndStepID { get; set; }
        public string Setting { get; set; }
        public int ResultID { get; set; }
        public string FlowchartPath { get; set; }
        public string FlowchartDirections { get; set; }
    }
}
