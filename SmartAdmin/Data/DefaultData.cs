using Smart.Core.Domain.Flow;
using Smart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.Data
{
    public class DefaultData
    {

        private IServices<Pipeline> _pipelineServices;
        private string businessEntityId;

        public DefaultData(string businessEntityId)
        {
            this.businessEntityId = businessEntityId;
        }

        public DefaultData(string id, IServices<Pipeline> pipelineServices)
        {
            this.Id = id;
            this._pipelineServices = pipelineServices;
        }

        public string Id { get; set; }

        public async void Pipeline()
        {

            var asfdsd = await _pipelineServices.GetAllAsync();



        }



    }
}
