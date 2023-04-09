﻿using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class EditService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message InputMessage { get; protected set; } = new Models.Message(Models.Role.Assistant, "What day of the wek is it?");

		public Models.Message InstructionMessage { get; protected set; } = new Models.Message { Content = "Fix the spelling mistakes" };

		public string Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.TextEditDavinciV1;

		public float? Temperature { get; set; }

		public int? ChoiceCount { get; set; }

		public float? TopP { get; set; }

		public EditService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<bool> Send(Models.Message inputMessage,Models.Message instructionMessage)
		{
			this.Messages.Add(inputMessage);
			this.Messages.Add(instructionMessage);

			var completionResult = await this.OpenAIService.Edit.CreateEdit(new OpenAI.GPT3.ObjectModels.RequestModels.EditCreateRequest()
			{
				Input = inputMessage.Content,
				Instruction = instructionMessage.Content,
				Model=this.Model,
				Temperature = this.Temperature,
				N = this.ChoiceCount,
				
				TopP = this.TopP,

			});

			if (completionResult.Successful)
				foreach (OpenAI.GPT3.ObjectModels.SharedModels.ChoiceResponse choice in completionResult.Choices)
					this.Messages.Add(new Models.Message(Models.Role.Server, choice.Text));
			else this.Messages.Add(new Models.Message(Models.Role.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}"));
			return completionResult.Successful;
		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.InputMessage, this.InstructionMessage))
            {
				this.InputMessage = new() { Role = this.InputMessage.Role };
				this.InstructionMessage = new() { Role = this.InstructionMessage.Role };
				return true;
            }
			return false;
		}
	}
}