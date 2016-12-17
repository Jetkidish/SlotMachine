using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
 * Slot Machine application, written by Tom Tsiliopoulos and Devon Cochrane (SN 200244662)
 * This app was created/updates on: December 14th, 2016
 * This is a basic slot machine emulator that accepts user bets, has a predefined amount of money, and uses random numbers to simulate chance
 */
namespace SlotMachine
{
    public partial class SlotMachineForm : Form
    {
        private int playerMoney = 1000;
        private int winnings = 0;
        private int jackpot = 5000;
        private float turn = 0.0f;
        private int playerBet = 0;
        private float winNumber = 0.0f;
        private float lossNumber = 0.0f;
        private string[] spinResult;
        private string fruits = "";
        private float winRatio = 0.0f;
        private float lossRatio = 0.0f;
        private int grapes = 0;
        private int bananas = 0;
        private int oranges = 0;
        private int cherries = 0;
        private int bars = 0;
        private int bells = 0;
        private int sevens = 0;
        private int blanks = 0;
        System.Media.SoundPlayer goodSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav");
        System.Media.SoundPlayer badSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
        System.Media.SoundPlayer spinSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");

        private Random random = new Random();

        public SlotMachineForm()
        {
            InitializeComponent();
            updateLabels();
        }

        /* Utility function to show Player Stats */
        private void showPlayerStats()
        {
            winRatio = winNumber / turn;
            lossRatio = lossNumber / turn;
            string stats = "";
            stats += ("Jackpot: " + jackpot + "\n");
            stats += ("Player Money: " + playerMoney + "\n");
            stats += ("Turn: " + turn + "\n");
            stats += ("Wins: " + winNumber + "\n");
            stats += ("Losses: " + lossNumber + "\n");
            stats += ("Win Ratio: " + (winRatio * 100) + "%\n");
            stats += ("Loss Ratio: " + (lossRatio * 100) + "%\n");
            MessageBox.Show(stats, "Player Stats");
        }

        /* Utility function to reset all fruit tallies*/
        private void resetFruitTally()
        {
            grapes = 0;
            bananas = 0;
            oranges = 0;
            cherries = 0;
            bars = 0;
            bells = 0;
            sevens = 0;
            blanks = 0;
        }

        /* Utility function to reset the player stats and forms */
        private void resetAll()
        {
            playerMoney = 1000;
            winnings = 0;
            jackpot = 5000;
            turn = 0;
            playerBet = 0;
            winNumber = 0;
            lossNumber = 0;
            winRatio = 0.0f;
            //the following lines were added by Devon - they clear the win/loss and payout labels, as well as setting the reel images to blanks
            WinLossLabel.Text = "";
            PayoutLabel.Text = "";
            reel1.Image = SlotMachine.Properties.Resources.blank;
            reel2.Image = SlotMachine.Properties.Resources.blank;
            reel3.Image = SlotMachine.Properties.Resources.blank;
        }

        /* Check to see if the player won the jackpot */
        private void checkJackPot()
        {
            /* compare two random values */
            var jackPotTry = this.random.Next(51) + 1;
            var jackPotWin = this.random.Next(51) + 1;
            if (jackPotTry == jackPotWin)
            {
                //rather than using a message box, we just update the appropriate labels
                WinLossLabel.Text = "JACKPOT!!";
                PayoutLabel.Text = ""+winnings+jackpot;
                playerMoney += jackpot;
                jackpot = 1000;
            }
        }

        /* Utility function to show a win message and increase player money */
        private void showWinMessage()
        {
            playerMoney += winnings;
            //message boxes have been replaced with labels
            WinLossLabel.Text = "WINNER!";
            PayoutLabel.Text = ""+winnings;
            resetFruitTally();
            checkJackPot();
            goodSoundPlayer.Play();
        }

        /* Utility function to show a loss message and reduce player money */
        private void showLossMessage()
        {
            //message boxes have been replaced with labels
            WinLossLabel.Text = "TRY AGAIN!";
            PayoutLabel.Text = "";
            playerMoney -= playerBet;
            resetFruitTally();
        }

        /* Utility function to check if a value falls within a range of bounds */
        private bool checkRange(int value, int lowerBounds, int upperBounds)
        {
            return (value >= lowerBounds && value <= upperBounds) ? true : false;
            
        }

        /* When this function is called it determines the betLine results.
    e.g. Bar - Orange - Banana */
        private string[] Reels()
        {
            string[] betLine = { " ", " ", " " };
            int[] outCome = { 0, 0, 0 };

            for (var spin = 0; spin < 3; spin++)
            {
                outCome[spin] = this.random.Next(65) + 1;

               if (checkRange(outCome[spin], 1, 27)) {  // 41.5% probability
                    betLine[spin] = "blank";
                    blanks++;
                    }
                else if (checkRange(outCome[spin], 28, 37)){ // 15.4% probability
                    betLine[spin] = "Grapes";
                    grapes++;
                }
                else if (checkRange(outCome[spin], 38, 46)){ // 13.8% probability
                    betLine[spin] = "Banana";
                    bananas++;
                }
                else if (checkRange(outCome[spin], 47, 54)){ // 12.3% probability
                    betLine[spin] = "Orange";
                    oranges++;
                }
                else if (checkRange(outCome[spin], 55, 59)){ //  7.7% probability
                    betLine[spin] = "Cherry";
                    cherries++;
                }
                else if (checkRange(outCome[spin], 60, 62)){ //  4.6% probability
                    betLine[spin] = "Bar";
                    bars++;
                }
                else if (checkRange(outCome[spin], 63, 64)){ //  3.1% probability
                    betLine[spin] = "Bell";
                    bells++;
                }
                else if (checkRange(outCome[spin], 65, 65)){ //  1.5% probability
                    betLine[spin] = "Seven";
                    sevens++;
                }

            }
            return betLine;
        }

        /* This function calculates the player's winnings, if any */
        private void determineWinnings()
        {
            if (blanks == 0)
            {
                if (grapes == 3)
                {
                    winnings = playerBet * 10;
                }
                else if (bananas == 3)
                {
                    winnings = playerBet * 20;
                }
                else if (oranges == 3)
                {
                    winnings = playerBet * 30;
                }
                else if (cherries == 3)
                {
                    winnings = playerBet * 40;
                }
                else if (bars == 3)
                {
                    winnings = playerBet * 50;
                }
                else if (bells == 3)
                {
                    winnings = playerBet * 75;
                }
                else if (sevens == 3)
                {
                    winnings = playerBet * 100;
                }
                else if (grapes == 2)
                {
                    winnings = playerBet * 2;
                }
                else if (bananas == 2)
                {
                    winnings = playerBet * 2;
                }
                else if (oranges == 2)
                {
                    winnings = playerBet * 3;
                }
                else if (cherries == 2)
                {
                    winnings = playerBet * 4;
                }
                else if (bars == 2)
                {
                    winnings = playerBet * 5;
                }
                else if (bells == 2)
                {
                    winnings = playerBet * 10;
                }
                else if (sevens == 2)
                {
                    winnings = playerBet * 20;
                }
                else if (sevens == 1)
                {
                    winnings = playerBet * 5;
                }
                else
                {
                    winnings = playerBet * 1;
                }
                winNumber++;
                showWinMessage();
            }
            else
            {
                lossNumber++;
                showLossMessage();
            }

        }

        private void SpinPictureBox_Click(object sender, EventArgs e)
        {
            //playerBet = 10; // default bet amount has been set to ZERO

            if (playerMoney == 0)
            {
                if (MessageBox.Show("You ran out of Money! \nDo you want to play again?","Out of Money!",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    resetAll();
                    showPlayerStats();
                }
            }
            else if (playerBet > playerMoney)
            {
                badSoundPlayer.Play();
                MessageBox.Show("You don't have enough Money to place that bet.", "Insufficient Funds");
                
            }
            else if (playerBet <= 0)
            {
                badSoundPlayer.Play();
                MessageBox.Show("All bets must be a positive $ amount.", "Incorrect Bet");
            }
            else if (playerBet <= playerMoney)
            {
                spinSoundPlayer.Play();
                spinResult = Reels();
                fruits = spinResult[0] + " - " + spinResult[1] + " - " + spinResult[2];
                updatePictureBoxes();
                determineWinnings();
                turn++;
                updateLabels();
                
                
            }
            else
            {
                badSoundPlayer.Play();
                MessageBox.Show("Please enter a valid bet amount");
                
            }
        }
        /*
         * The following event handlers are for the bet "buttons" and their corresponding values
         */
        private void Bet1PictureBox_Click(object sender, EventArgs e)
        {
            playerBet = 1;
            updateLabels();
        }

        private void Bet5PictureBox_Click(object sender, EventArgs e)
        {
            playerBet = 5;
            updateLabels();
        }

        private void Bet10PictureBox_Click(object sender, EventArgs e)
        {
            playerBet = 10;
            updateLabels();
        }

        private void Bet25PictureBox_Click(object sender, EventArgs e)
        {
            playerBet = 25;
            updateLabels();
        }
        /* This function will update the reels based on the results */
        private void updatePictureBoxes()
        {
            //reel 1
            if (spinResult[0] == "Grapes")
            {
                reel1.Image = SlotMachine.Properties.Resources.grapes;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Banana")
            {
                reel1.Image = SlotMachine.Properties.Resources.banana;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Orange")
            {
                reel1.Image = SlotMachine.Properties.Resources.orange;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Cherry")
            {
                reel1.Image = SlotMachine.Properties.Resources.cherry;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Bar")
            {
                reel1.Image = SlotMachine.Properties.Resources.bar;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Bell")
            {
                reel1.Image = SlotMachine.Properties.Resources.bell;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "Seven")
            {
                reel1.Image = SlotMachine.Properties.Resources.seven;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[0] == "blank")
            {
                reel1.Image = SlotMachine.Properties.Resources.blank;
                reel1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            //----reel 2----
            if (spinResult[1] == "Grapes")
            {
                reel2.Image = SlotMachine.Properties.Resources.grapes;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Banana")
            {
                reel2.Image = SlotMachine.Properties.Resources.banana;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Orange")
            {
                reel2.Image = SlotMachine.Properties.Resources.orange;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Cherry")
            {
                reel2.Image = SlotMachine.Properties.Resources.cherry;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Bar")
            {
                reel2.Image = SlotMachine.Properties.Resources.bar;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Bell")
            {
                reel2.Image = SlotMachine.Properties.Resources.bell;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "Seven")
            {
                reel2.Image = SlotMachine.Properties.Resources.seven;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[1] == "blank")
            {
                reel2.Image = SlotMachine.Properties.Resources.blank;
                reel2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            //----reel 3----
            if (spinResult[2] == "Grapes")
            {
                reel3.Image = SlotMachine.Properties.Resources.grapes;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Banana")
            {
                reel3.Image = SlotMachine.Properties.Resources.banana;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Orange")
            {
                reel3.Image = SlotMachine.Properties.Resources.orange;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Cherry")
            {
                reel3.Image = SlotMachine.Properties.Resources.cherry;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Bar")
            {
                reel3.Image = SlotMachine.Properties.Resources.bar;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Bell")
            {
                reel3.Image = SlotMachine.Properties.Resources.bell;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "Seven")
            {
                reel3.Image = SlotMachine.Properties.Resources.seven;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (spinResult[2] == "blank")
            {
                reel3.Image = SlotMachine.Properties.Resources.blank;
                reel3.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        /*
         * This will update all the labels in the form, as well as check if the spin button should be disabled
         */
        private void updateLabels()
        {
            MoneyLabel.Text = "" + playerMoney;
            BetLabel.Text = "" + playerBet;
            JackpotLabel.Text = "Jackpot: " + jackpot;
            if (playerBet == 0)
            {
                SpinPictureBox.Image = SlotMachine.Properties.Resources.spin_disabled;
            }
            else if(playerBet > 0)
            {
                SpinPictureBox.Image = SlotMachine.Properties.Resources.spin;
            }
        }
        /*
         * power/quit button; will exit the application when clicked
         */
        private void quitPictureBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /*
         * reset button; will update all labels and picture boxes, as well as clear player stats, etc
         */
        private void resetPictureBox_Click(object sender, EventArgs e)
        {
            resetAll();
            updateLabels();
        }
        /*
         * This will bring up the message box to show the player stats
         */
        private void StatsPictureBox_Click(object sender, EventArgs e)
        {
            showPlayerStats();
        }
    }

}
