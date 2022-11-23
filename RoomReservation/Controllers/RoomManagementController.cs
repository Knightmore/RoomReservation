#region Copyright © 2022 Patrick Borger - https: //github.com/Knightmore

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Author: Patrick Borger
// GitHub: https://github.com/Knightmore
// Created: 10.10.2022
// Modified: 20.10.2022

#endregion

using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace RoomReservation.Controllers;

public class RoomManagementController : DatabaseController
{
    private readonly IStringLocalizer<RoomManagementController> _localizer;

    public RoomManagementController(IStringLocalizer<RoomManagementController> localizer)
    {
        _localizer = localizer;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        List<SeatViewModel> seats = await DbContext.Rooms.Join(DbContext.Seats, room => room.RoomId, seat => seat.RoomId, (room, seat) => new SeatViewModel
                                                                                                                                          {
                                                                                                                                              Id        = seat.id,
                                                                                                                                              Title     = seat.title,
                                                                                                                                              ExtraInfo = seat.ExtraInfo,
                                                                                                                                              RoomId    = room.RoomId,
                                                                                                                                              RoomName  = room.Name,
                                                                                                                                              RoomLimit = room.Limit
                                                                                                                                          }).ToListAsync();
        Dictionary<int, List<SeatViewModel>> groupedSeats = seats.OrderBy(x => x.Title).GroupBy(x => x.RoomId).ToDictionary(room => room.Key, room => room.ToList());
        foreach (Room room in DbContext.Rooms)
            if (!groupedSeats.ContainsKey(room.RoomId))
                groupedSeats.Add(room.RoomId, new List<SeatViewModel> { new() { RoomId = room.RoomId, RoomName = room.Name } });


        return View(groupedSeats.OrderBy(x => x.Key).ToDictionary(room => room.Key, room => room.Value));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoom(string name, int limit)
    {
        if (string.IsNullOrEmpty(name) || limit < 0)
        {
            TempData["errors"] = _localizer["Roomname or limit invalid!"].ToString();
            return RedirectToAction("Index");
        }

        if (await DbContext.Rooms.FirstOrDefaultAsync(room => room.Name == name) != null)
        {
            TempData["success"] = _localizer["Room {0} already exists!", name].ToString();
            return RedirectToAction("Index");
        }

        await DbContext.Rooms.AddAsync(new Room { Name = name, Limit = limit });
        await DbContext.SaveChangesAsync();
        TempData["success"] = _localizer["Room {0} successfully added!", name].ToString();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ChangeLimit(int id, int limit)
    {
        if (id <= 0 || limit < 0)
        {
            TempData["errors"] = _localizer["RoomId or limit invalid!"].ToString();
            return RedirectToAction("Index");
        }

        Room? room = await DbContext.Rooms.FirstOrDefaultAsync(room => room.RoomId == id);
        if (room == null) return RedirectToAction("Index");
        room.Limit = limit;
        DbContext.Rooms.Update(room);
        await DbContext.SaveChangesAsync();
        TempData["success"] = HtmlEncoder.Default.Encode(_localizer["Limit for {0} successfully changed to {1}!", room.Name, limit].ToString());
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ChangeRoomName(int id, string name)
    {
        if (id <= 0 || string.IsNullOrEmpty(name))
        {
            TempData["errors"] = _localizer["RoomId or name invalid!"].ToString();
            return RedirectToAction("Index");
        }

        Room? room = await DbContext.Rooms.FirstOrDefaultAsync(room => room.RoomId == id);
        if (room == null) return RedirectToAction("Index");
        room.Name = name;
        DbContext.Rooms.Update(room);
        await DbContext.SaveChangesAsync();
        TempData["success"] = HtmlEncoder.Default.Encode(_localizer["Name for {0} successfully changed to {1}!", room.RoomId, name].ToString());
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
        Room? roomToDelete = await DbContext.Rooms.FirstOrDefaultAsync(room => room.RoomId == int.Parse(id));
        if (roomToDelete == null) return RedirectToAction("Index");
        string roomName = roomToDelete.Name;
        DbContext.Rooms.Remove(roomToDelete);
        await DbContext.SaveChangesAsync();
        TempData["success"] = _localizer["Room {0} successfully deleted!", roomName].ToString();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSeat(string name, int roomId, string roomName, string extraInfo)
    {
        if (string.IsNullOrEmpty(name))
        {
            TempData["errors"] = _localizer["Seatname can not be empty!"].ToString();
            return RedirectToAction("Index");
        }

        if (await DbContext.Seats.FirstOrDefaultAsync(seat => seat.title == name && seat.RoomId == roomId) != null)
        {
            TempData["success"] = _localizer["Seat {0} in room {1} already exists!", name, roomName].ToString();
            return RedirectToAction("Index");
        }

        int       nextId;
        List<int> roomSeats = DbContext.Seats.Where(seat => seat.RoomId == roomId).Select(seat => seat.id).ToList().Select(seat => int.Parse(seat.Substring(seat.LastIndexOf('-') + 1))).OrderBy(id => id).ToList();
        if (roomSeats.Any())
        {
            if (roomSeats.Last() == roomSeats.Count())
                nextId = roomSeats.Count() + 1;
            else
                nextId = Enumerable.Range(1, roomSeats.Last()).Except(roomSeats).First();
        }
        else
        {
            nextId = 1;
        }

        DbContext.Seats.Add(new Seat { id = $"{roomName}-{roomId}-{nextId.ToString()}", title = name, RoomId = roomId, ExtraInfo = extraInfo });
        await DbContext.SaveChangesAsync();
        TempData["success"] = _localizer["Seat {0} for Room {1} successfully added!", name, roomName].ToString();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSeat(string id, string seatName, string roomName)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(seatName) || string.IsNullOrEmpty(roomName)) return RedirectToAction("Index");
        Seat? seatToDelete = await DbContext.Seats.FirstOrDefaultAsync(seat => seat.id == id);
        if (seatToDelete == null) return RedirectToAction("Index");
        DbContext.Seats.Remove(seatToDelete);
        await DbContext.SaveChangesAsync();
        TempData["success"] = _localizer["Seat {0} in Room {1} successfully deleted!", seatName, roomName].ToString();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSeat(string id, string roomName, string seatName, string additionalInfo)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(seatName)) return RedirectToAction("Index");
        Seat? seat = await DbContext.Seats.FirstOrDefaultAsync(seat => seat.id == id);
        if (seat == null) return RedirectToAction("Index");
        seat.title     = seatName;
        seat.ExtraInfo = additionalInfo;
        DbContext.Seats.Update(seat);
        await DbContext.SaveChangesAsync();
        TempData["success"] = _localizer["Seat {0} in Room {1} successfully updated!", seatName, roomName].ToString();
        return RedirectToAction("Index");
    }
}