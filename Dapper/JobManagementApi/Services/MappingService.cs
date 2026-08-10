using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Auth;
using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Entities;
using JobManagementApi.Enums;
using JobManagementApi.Exceptions;
using JobManagementApi.Extensions;
using JobManagementApi.Interfaces;

namespace JobManagementApi.Services;

public class MappingService(DapperContext context) : IMappingService
{
    public async Task<JobApplicationDetailsDto> GetApplicationDetails(int applicationId)
    {
        using var connection=context.GetConnection();
        string query=@"
            select a.*,j.*,r.*,c.*,comp.*
            from application a left join job j
            on a.jobid=j.id

            left join user r
            on j.RecruiterId=r.Id

            left join user c
            on a.CandidateId=c.Id

            left join company comp
            on j.CompanyId=comp.Id

            WHERE a.Id=@id;
        ";

        var result=(await connection.QueryAsync<Application,Job,User,User,Company,Application>(
            query,
            (application,job,recruiter,candidate,company) =>
            {
                if (application.Id != 0)
                {
                    application.Job=job;
                    application.Candidate=candidate;
                    application.Job.Company=company;
                    application.Job.Recruiter=recruiter;
                }
                return application;
            },
            new{id=applicationId},
            splitOn:"Id,Id,Id,Id"
        )).ToList();
        if(result.Count==0) throw new NotFoundException("application not found");
        var application=result.First();

        var candidate=new UserSummaryDto(application.Candidate.Id,application.Candidate.Name,application.Candidate.Email,
                            application.Candidate.Role);
            
        var recruiter=new UserSummaryDto(application.Job.Recruiter.Id,application.Job.Recruiter.Name,application.Job.Recruiter.Email,
                        application.Job.Recruiter.Role);
        var company=new CompanyDto(application.Job.Company.Id,application.Job.Company.UserId,application.Job.Company.Name,
                        application.Job.Company.Description,application.Job.Company.Location,application.Job.Company.Website);
        var job=new JobDetailsDto(application.Job.Id,application.Job.Title,application.Job.Description,application.Job.Location,
                    application.Job.SalaryMin,application.Job.SalaryMax,application.Job.JobType,application.Job.Experience,
                    company,recruiter);
            
        return new(application.Id,application.ResumeUrl,application.Status,job,candidate); 
    }

    public async Task<CompanyDetailsDto> GetCompanyDetails(int companyId)
    {
        using var connection=context.GetConnection();
        var query=@"select c.*,j.*
                    from company c left join job j
                    on c.Id=j.companyId
                    where c.Id=@id";

        Dictionary<int,Company>companyDictionary=[];
        await connection.QueryAsync<Company,Job,Company>(
            query,
            (company,job) =>
            {
                if(!companyDictionary.TryGetValue(company.Id,out var existingcompany))
                {
                    existingcompany=company;
                    existingcompany.Jobs=[];
                    companyDictionary.Add(company.Id,existingcompany);
                }

                if(job is not null && job.Id!=0 && !existingcompany.Jobs.Any(x => x.Id == job.Id))
                {
                    existingcompany.Jobs.Add(job);
                }
                return existingcompany;
            },
            new{id=companyId},
            splitOn:"Id"
        );
        if(companyDictionary.Count==0) throw new NotFoundException("Company not found");
        var company=companyDictionary.Values.First();

        return new(company.Id,company.UserId,company.Name,company.Description,company.Location,company.Website,
                    company.Jobs.Select(x=>x.ToDto()));
    }

    public async Task<JobDetailsDto> GetJobDetails(int jobId)
    {
        using var connection=context.GetConnection();
        string query=@"select j.*,c.*,r.*
                    
                    from job j left join company c
                    on j.companyid=c.Id
                    join User r
                    on  j.RecruiterId=r.Id 
                    where j.Id=@id;
                    ";

        var result=(await connection.QueryAsync<Job,Company,User,Job>(
            query,
            (job,company,user) =>
            {
                if (job.Id != 0)
                {
                    job.Company=company;
                    job.Recruiter=user;
                }
                return job;
            },
            new{id=jobId},
            splitOn:"Id,Id"
        )).ToList();
        if(result.Count==0) throw new NotFoundException("job not found");
        var job=result.First();
        var company=new CompanyDto(job.Company.Id,job.Company.UserId,job.Company.Name,job.Company.Description,
                                    job.Company.Location,job.Company.Website);
        var recruiter=new UserSummaryDto(job.Recruiter.Id,job.Recruiter.Name,job.Recruiter.Email,job.Recruiter.Role);

        return new(job.Id,job.Title,job.Description,job.Location,job.SalaryMin,job.SalaryMax,job.JobType,job.Experience,
                    company,
                    recruiter);
    }

    public async Task<JobWithSkillsDetailsDto> GetJobWithSkillsDetails(int jobId)
    {
        using var connection=context.GetConnection();
        string query=@"select j.*,c.*,r.*,s.* from
                        
                        job j left join jobskills js
                        on js.JobId=j.Id
                        
                        left join skills s
                        on js.skillid=s.id

                        inner join company c
                        on c.id=j.companyid

                        inner join user r
                        on j.RecruiterId=r.Id

                        where j.Id=@id
                        ";

        List<Skills>skillsRequired=[];
        var result=(await connection.QueryAsync<Job,Company,User,Skills,Job>(
            query,
            (job,company,recruiter,skills) =>
            {
                job.Company=company;
                job.Recruiter=recruiter;
                if (skills is not null && skills.Id!=0 && !skillsRequired.Any(x => x.Id == skills.Id))
                {
                    skillsRequired.Add(skills);
                }
                return job;
            },
            new{id=jobId},
            splitOn:"id,id,id"
        )).ToList();
        if(result.Count==0) throw new Exception("job not found");
        var job=result.First();

        var company=new CompanyDto(job.Company.Id,job.Company.UserId,job.Company.Name,job.Company.Description,job.Company.Location,
                        job.Company.Website);

        var recruiter=new UserSummaryDto(job.Recruiter.Id,job.Recruiter.Name,job.Recruiter.Email,job.Recruiter.Role);

        return new(job.Id,job.Title,job.Description,job.Location,job.SalaryMin,job.SalaryMax,job.JobType,job.Experience,company,
                recruiter,skillsRequired.Select(x=>x.ToDto()));
    }
}