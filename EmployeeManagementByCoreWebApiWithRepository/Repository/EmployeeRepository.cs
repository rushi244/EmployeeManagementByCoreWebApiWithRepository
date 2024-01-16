using EmployeeManagementByCoreWebApiWithRepository.DataAccessLayer;
using EmployeeManagementByCoreWebApiWithRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Project.Shared;
using System;

namespace EmployeeManagementByCoreWebApiWithRepository.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Employee> AddEmployee(Employee employee)
        {
            if(employee.Department != null)
            {
                dbContext.Entry(employee.Department).State = EntityState.Unchanged;
            }

           var result= await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteEmployee(int employeeId)
        {
            var result = await dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (result != null)
            {
                dbContext.Employees.Remove(result);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await dbContext.Employees
                 .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
           return await dbContext.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> Search(string name, Gender? gender)
        {
            IQueryable<Employee> query = dbContext.Employees;
            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
            }
            if(gender != null)
            {
                query = query.Where(e => e.Gender == gender);
            }
            return await query.ToListAsync();
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result = await dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
            if(result != null)
            {
                result.FirstName = employee.FirstName;
                result.LastName = employee.LastName;
                result.Email = employee.Email;
                result.DateOfBrith = employee.DateOfBrith;
                result.Gender = employee.Gender;
                if(employee.DepartmentId!=null)
                {
                    result.DepartmentId = employee.DepartmentId;
                }
                result.PhotoPath = employee.PhotoPath;

                await dbContext.SaveChangesAsync();

                return result;

            }
            return null;
        }
    }
}
