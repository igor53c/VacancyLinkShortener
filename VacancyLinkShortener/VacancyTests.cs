using VacancyLinkShortenerCsharpImplementation;
using Xunit;

public class VacancyTests
{
    [Fact]
    public void TestStellenchaOsVacancy()
    {
        var vacancy = new VacancyLinkShortenerCsharp.Vacancy(12345, "Junior Software Developer (m/w/d) - 100% remote", "Software & More Inc.", new[] { "München", "Dresden", "Berlin" }, VacancyLinkShortenerCsharp.Platform.STELLANCHA_OS);
        var siteContent = vacancy.GetSiteContent();

        Assert.Equal("https://www.stellencha.os/stellen/software-more-inc/12345/junior-software-developer-m-w-d-100-remote/", siteContent.Url);
        Assert.Equal("Junior Software Developer (m/w/d) - 100% remote | Software & More Inc. | München, Dresden, Berlin | stellencha.os", siteContent.Title);
    }

    [Fact]
    public void TestJobsMitBizVacancy()
    {
        var vacancy = new VacancyLinkShortenerCsharp.Vacancy(12345, "Junior Software Developer (m/w/d) - 100% remote", "Software & More Inc.", new[] { "München", "Dresden", "Berlin" }, VacancyLinkShortenerCsharp.Platform.JOBS_MIT_BIZ);
        var siteContent = vacancy.GetSiteContent();

        Assert.Equal("https://www.jobs-mit.biz/Jobs/Muenchen-Dresden-Berlin/12345/Junior-Software-Developer-m-w-d-100-remote/", siteContent.Url);
        Assert.Equal("Jetzt Bewerben! - Junior Software Developer (m/w/d) - 100% remote @ München, Dresden und Berlin", siteContent.Title);
    }

    [Fact]
    public void TestJobDealerVacancy()
    {
        var vacancy = new VacancyLinkShortenerCsharp.Vacancy(12345, "Junior Software Developer (m/w/d) - 100% remote", "Software & More Inc.", new[] { "München", "Dresden", "Berlin" }, VacancyLinkShortenerCsharp.Platform.JOB_DEALER);
        var siteContent = vacancy.GetSiteContent();

        Assert.Equal("https://www.job.dealer/job/12345_junior_software_developer_m_w_d_100_remote_bei_software_more_inc_in_munchen_dresden_berlin", siteContent.Url);
        Assert.Equal("Junior Software Developer (m/w/d) - 100% remote bei Software & More Inc. in München, Dresden oder Berlin von deinem job.dealer", siteContent.Title);
    }
}
