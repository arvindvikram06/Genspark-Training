using BLLibrary.Services;
using DALLibrary.Contexts;
using DALLibrary.Repositories;
using FELibrary.Admin;
using FELibrary.Members;
using FELibrary.Menus;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("===Welcome===");

var context = new BookRentalContext();
var memberRepository = new MemberRepository(context);
var membershipRepository = new MembershipRepository(context);
var bookRepository = new BookRepository(context);
var bookCopyRepository = new BookCopyRepository(context);
var borrowingRepository = new BorrowingRepository(context);
var fineRepository = new FineRepository(context);
var damageRepository = new BookDamageHistoryRepository(context);
var categoryRepository = new BookCategoryRepository(context);

var memberService = new MemberService(memberRepository, context);
var membershipService = new MembershipService(membershipRepository, context);
var bookService = new BookService(bookRepository, bookCopyRepository, damageRepository, fineRepository, borrowingRepository, context);
var fineService = new FineService(fineRepository, context);
var borrowingService = new BorrowingService(borrowingRepository, memberRepository, bookCopyRepository, fineRepository, context);
var categoryService = new CategoryService(categoryRepository, context);
var reportRepository = new ReportRepository(context);
var reportService = new ReportService(reportRepository);

var memmberMenu = new MemberMenu(memberService);
var adminMenu = new AdminMenu(membershipService, bookService, reportService, memmberMenu, categoryService);
var loggedInMemberMenu = new LoggedInMemberMenu(memberService, borrowingService, bookService, fineService, membershipService);

var loginMenu = new LoginMenu(memberService, adminMenu, loggedInMemberMenu);
loginMenu.Show();


Console.WriteLine("Goodbye!");

