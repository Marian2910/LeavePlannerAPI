using AutoMapper;
using Domain.Models;
using Domain.Services;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.PersonalEventTests
{
    [TestFixture]
    public class PersonalEventServiceTests
    {
        private PersonalEventService _personalEventService;
        private Mock<IPersonalEventRepository> _personalEventRepository;
        private Mock<IEmployeeRepository> _employeeRepository;
        private Mock<IMapper> _mapper;
        private Mock<ILogger<PersonalEventService>> _logger;

        [OneTimeSetUp]
        public void SetUp()
        {
            _personalEventRepository = new Mock<IPersonalEventRepository>();
            _employeeRepository = new Mock<IEmployeeRepository>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<PersonalEventService>>();
            _personalEventService = new PersonalEventService(_personalEventRepository.Object, _employeeRepository.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task AddPersonalEvent()
        {
            //act
            var employeeId = 2;
            var personalEvent = new PersonalEvent { EmployeeId = employeeId };
            var existingEmployee = new Infrastructure.Entities.Employee { Id = employeeId };
            var mappedPersonalEvent = new Infrastructure.Entities.PersonalEvent();

            _employeeRepository.Setup(repo => repo.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mapper.Setup(mapper => mapper.Map<Infrastructure.Entities.PersonalEvent>(personalEvent)).Returns(mappedPersonalEvent);

            //act
            await _personalEventService.AddEventAsync(personalEvent);

            //assert
            _personalEventRepository.Verify(repo => repo.AddPersonalEventAsync(mappedPersonalEvent));
        }

        [Test]
        public async Task GetAllPersonalEvents()
        {
            //act
            var personalEventEntities = new List<Infrastructure.Entities.PersonalEvent>();
            {
                new Infrastructure.Entities.PersonalEvent
                {
                    Id = 1,
                    Title = "Birthday",
                    Description = "Birthday of Jane Doe",
                    StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                    EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                    Location = "Conference Room",
                    Employee = new Infrastructure.Entities.Employee { Id = 2 }
                };
            };
             var personalEvents = new List<PersonalEvent> 
             {
                new PersonalEvent
                    {
                     Id = 1,
                     Title = "Birthday",
                     Description = "Birthday of Jane Doe",
                     StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                     EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                     Location = "Conference Room",
                     EmployeeId = 2
                    }
             };

             _personalEventRepository.Setup(repo => repo.GetAllPersonalEventsAsync()).ReturnsAsync(personalEventEntities);
             _mapper.Setup(mapper =>mapper.Map<IEnumerable<PersonalEvent>>(personalEventEntities)).Returns(personalEvents);

             //act
             var result = await _personalEventService.GetAllPersonalEventsAsync();

            //assert
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetPersonalEventById()
        {
            //arrange
            var eventId = 1;
            var personalEventEntities = new Infrastructure.Entities.PersonalEvent
            {
                Id = eventId,
                Title = "Birthday",
                Description = "Birthday of Jane Doe",
                StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                Location = "Conference Room",
                Employee = new Infrastructure.Entities.Employee { Id = 2 }
            };
            var personalEvent = new PersonalEvent
            {
                Id = eventId,
                Title = "Birthday",
                Description = "Birthday of Jane Doe",
                StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                Location = "Conference Room",
                EmployeeId = 2
            };

            _personalEventRepository.Setup(repo => repo.GetPersonalEventByIdAsync(eventId)).ReturnsAsync(personalEventEntities);
            _mapper.Setup(mapper => mapper.Map<PersonalEvent>(personalEventEntities)).Returns(personalEvent);

            //act
            var result = await _personalEventService.GetPersonalEventByIdAsync(eventId);

            //assert
            Assert.That(result, Is.EqualTo(personalEvent));
        }

        [Test]
        public async Task GetPersonalEventsByEmployeeId()
        {
            //assert
            var employeeId = 1;
            var personalEventEntities = new List<Infrastructure.Entities.PersonalEvent>
            {
                new Infrastructure.Entities.PersonalEvent
                {
                Id = 1,
                Title = "Birthday",
                Description = "Birthday of Jane Doe",
                StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                Location = "Conference Room",
                Employee = new Infrastructure.Entities.Employee { Id = employeeId }
                }
            };
            var personalEvent = new List<PersonalEvent>
            {
                new PersonalEvent
                {
                Id = 1,
                Title = "Birthday",
                Description = "Birthday of Jane Doe",
                StartDate = new DateTime(2023, 11, 20, 10, 0, 0),
                EndDate = new DateTime(2023, 11, 20, 22, 0, 0),
                Location = "Conference Room",
                EmployeeId = employeeId
                }
            };
            _personalEventRepository.Setup(repo => repo.GetPersonalEventsByEmployeeIdAsync(employeeId)).ReturnsAsync(personalEventEntities);
            _mapper.Setup(mapper => mapper.Map<IEnumerable<PersonalEvent>>(personalEventEntities)).Returns(personalEvent);

            //act
            var result = await _personalEventService.GetPersonalEventsByEmployeeId(employeeId);

            //assert
            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
