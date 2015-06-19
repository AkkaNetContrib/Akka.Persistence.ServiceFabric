//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Persistence.ServiceFabric.Tests;
using System;
using System.Linq.Expressions;

namespace Akka.Persistence.ServiceFabric.X64TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            RunJournal(test => test.Journal_should_not_replay_messages_if_count_limit_equals_zero());
            RunJournal(test => test.Journal_should_not_replay_messages_if_lower_sequence_number_bound_is_greater_than_upper_sequence_number_bound());
            RunJournal(test => test.Journal_should_replay_all_messages());
            RunJournal(test => test.Journal_should_replay_a_single_if_lower_sequence_number_bound_equals_upper_sequence_number_bound());
            RunJournal(test => test.Journal_should_replay_a_single_message_if_count_limit_is_equal_one());
            RunJournal(test => test.Journal_should_replay_logically_deleted_messages_with_deleted_flag_set_on_range_deletion());
            RunJournal(test => test.Journal_should_replay_messages_using_an_upper_sequence_number_bound());
            RunJournal(test => test.Journal_should_replay_messages_using_a_count_limit());
            RunJournal(test => test.Journal_should_replay_messages_using_a_lower_sequence_number_bound());
            RunJournal(test => test.Journal_should_replay_messages_using_lower_and_upper_sequence_number_bound());
            RunJournal(test => test.Journal_should_replay_messages_using_lower_and_upper_sequence_number_bound_and_count_limit());
            RunJournal(test => test.Journal_should_return_a_highest_sequence_number_equal_zero_if_actor_did_not_written_any_messages_yet());
            RunJournal(test => test.Journal_should_return_a_highest_seq_number_greater_than_zero_if_actor_has_already_written_messages_and_message_log_is_not_empty());

            RunSnapshot(test => test.SnapshotStore_should_delete_all_snapshots_matching_upper_sequence_number_and_timestamp_bounds());
            RunSnapshot(test => test.SnapshotStore_should_delete_a_single_snapshot_identified_by_snapshot_metadata());
            RunSnapshot(test => test.SnapshotStore_should_load_a_most_recent_snapshot());
            RunSnapshot(test => test.SnapshotStore_should_load_a_most_recent_snapshot_matching_an_upper_sequence_number_and_timestamp_bound());
            RunSnapshot(test => test.SnapshotStore_should_load_a_most_recent_snapshot_matching_an_upper_sequence_number_bound());
            RunSnapshot(test => test.SnapshotStore_should_not_load_a_snapshot_given_an_invalid_persistence_id());
            RunSnapshot(test => test.SnapshotStore_should_not_load_a_snapshot_given_non_matching_sequence_number_criteria());
            RunSnapshot(test => test.SnapshotStore_should_not_load_a_snapshot_given_non_matching_timestamp_criteria());

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("All tests completed.");
            Console.ReadLine();
        }

        private static void RunJournal(Expression<Action<ServiceFabricJournalSpec>> x)
        {
            Console.ForegroundColor = ConsoleColor.White;
            var name = x.Body.ToString();
            var action = x.Compile();
            try
            {
                var test = new ServiceFabricJournalSpec();
                action(test);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} Success..",name);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} Failed..",name);
            }
        }

        private static void RunSnapshot(Expression<Action<ServiceFabricSnapshotStoreSpec>> x)
        {
            Console.ForegroundColor = ConsoleColor.White;
            var name = x.Body.ToString();
            var action = x.Compile();
            try
            {
                var test = new ServiceFabricSnapshotStoreSpec();
                action(test);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} Success..", name);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} Failed..", name);
            }
        }
    }
}
